import pandas as pd
import os
from sqlalchemy import create_engine
from datetime import datetime
import json

script_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, 'Config.json')
with open(config_path, 'r') as file:
    creds = json.load(file)

conn_str = f"mysql+mysqldb://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
engine = create_engine(conn_str)

def analysis_risk_churn(file_path):
    data = pd.read_csv(file_path)
    data['FECHA'] = pd.to_datetime(data['FECHA'], errors='coerce')
    data = data.dropna(subset=['FECHA'])
    data['Recencia'] = (datetime.now() - data.groupby('NOMBRE_CLIENTE')['FECHA'].transform('max')).dt.days
    data['Churn'] = data['Recencia'] > 180
    data.to_sql('cliente_churn', engine, if_exists='replace', index=False)
    #print("Datos de churn procesados y guardados en la base de datos")
