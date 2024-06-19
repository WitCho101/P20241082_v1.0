import pandas as pd
from sqlalchemy import create_engine
import json
import os

script_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, 'Config.json')
with open(config_path, 'r') as file:
    creds = json.load(file)

conn_str = f"mysql+pymysql://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
engine = create_engine(conn_str)

def analysis_cohortes(file_path):
    data = pd.read_csv(file_path)
    data['FECHA'] = pd.to_datetime(data['FECHA'])
    data['Cohorte'] = data.groupby('NOMBRE_CLIENTE')['FECHA'].transform('min').dt.to_period('M')
    data.to_sql('ventas_cohortes', engine, if_exists='replace', index=False)
