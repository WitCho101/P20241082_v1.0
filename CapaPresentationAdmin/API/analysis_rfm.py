import pandas as pd
import json
import os
from sqlalchemy import create_engine
import datetime as dt

script_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, 'Config.json')
with open(config_path, 'r') as file:
    creds = json.load(file)

conn_str = f"mysql+mysqldb://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
engine = create_engine(conn_str)

def analisis_rfm(file_path):
    data = pd.read_csv(file_path)
    data['FECHA'] = pd.to_datetime(data['FECHA'], errors='coerce')
    fecha_ref = data['FECHA'].max() + dt.timedelta(days=1)
    rfm = data.groupby('NOMBRE_CLIENTE').agg({
        'FECHA': lambda x: (fecha_ref - x.max()).days,
        'NRO_DOCUMENTO': 'count',
        'VALORSINIGV': 'sum'
    }).reset_index()
    rfm.columns = ['NOMBRE_CLIENTE', 'RECENCIA', 'FRECUENCIA', 'VALOR_MONETARIO']
    rfm.to_sql('rfm_analisis', engine, if_exists='replace', index=False)
