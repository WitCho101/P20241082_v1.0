import pandas as pd
import json
import os
from sqlalchemy import create_engine

script_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, 'Config.json')
with open(config_path, 'r') as file:
    creds = json.load(file)

conn_str = f"mysql+mysqldb://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
engine = create_engine(conn_str)

def calcular_satisfaccion(file_path):
    data = pd.read_csv(file_path)
    
    data['FECHA'] = pd.to_datetime(data['FECHA'], errors='coerce')
    satisfaction_data = data.groupby('NOMBRE_CLIENTE').agg({
        'VALORSINIGV': 'sum',
        'NRO_DOCUMENTO': 'count'
    }).reset_index()
    
    satisfaction_data['Satisfaccion'] = (satisfaction_data['VALORSINIGV'] / satisfaction_data['VALORSINIGV'].max()) * 50 + \
                                        (satisfaction_data['NRO_DOCUMENTO'] / satisfaction_data['NRO_DOCUMENTO'].max()) * 50
    
    satisfaction_data.to_sql('cliente_satisfaccion', engine, if_exists='replace', index=False)
