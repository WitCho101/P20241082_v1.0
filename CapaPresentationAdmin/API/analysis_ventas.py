import pandas as pd
import os
from sqlalchemy import create_engine
from sqlalchemy.exc import SQLAlchemyError
from sklearn.preprocessing import StandardScaler
from sklearn.cluster import KMeans
import json

script_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, 'Config.json')
with open(config_path, 'r') as file:
    creds = json.load(file)

conn_str = f"mysql+mysqldb://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
engine = create_engine(conn_str)

def analisis_ventas(file_path):
    data = pd.read_csv(file_path)
    cliente_ventas = data.groupby(['NOMBRE_CLIENTE']).agg({
        'NRO_DOCUMENTO': 'count',
        'VALORSINIGV': 'sum'
    }).reset_index()
    cliente_ventas.columns = ['NOMBRE_CLIENTE', 'FRECUENCIA_COMPRA', 'VALOR_TOTAL']
    cliente_ventas.to_sql('cliente_ventas', engine, if_exists='replace', index=False)
    scaler = StandardScaler()
    scaled_features = scaler.fit_transform(cliente_ventas[['FRECUENCIA_COMPRA', 'VALOR_TOTAL']])
    kmeans = KMeans(n_clusters=4)
    cliente_ventas['CLUSTER'] = kmeans.fit_predict(scaled_features)
    cliente_ventas.to_sql('cliente_segmentado', engine, if_exists='replace', index=False)
