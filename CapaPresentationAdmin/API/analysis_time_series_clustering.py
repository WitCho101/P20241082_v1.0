import pandas as pd
import json
import os
from sqlalchemy import create_engine
from tslearn.clustering import TimeSeriesKMeans

script_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, 'Config.json')
with open(config_path, 'r') as file:
    creds = json.load(file)

conn_str = f"mysql+mysqldb://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
engine = create_engine(conn_str)

def analisis_time_series_clustering(file_path):
    data = pd.read_csv(file_path)
    data['FECHA'] = pd.to_datetime(data['FECHA'], errors='coerce')
    subset_data = data[(data['FECHA'] >= '2023-01-01') & (data['FECHA'] <= '2023-01-31')]
    subset_data = subset_data[subset_data['NOMBRE_CLIENTE'].isin(subset_data['NOMBRE_CLIENTE'].unique()[:50])]
    ts_data = subset_data.pivot_table(index='FECHA', columns='NOMBRE_CLIENTE', values='VALORSINIGV', aggfunc='sum').fillna(0)
    ts_data_array = ts_data.values.T
    model = TimeSeriesKMeans(n_clusters=3, metric="dtw", random_state=42)
    ts_clusters = model.fit_predict(ts_data_array)
    cliente_cluster = pd.DataFrame({'NOMBRE_CLIENTE': ts_data.columns, 'TS_CLUSTER': ts_clusters})
    cliente_cluster.to_sql('ts_clusters', engine, if_exists='replace', index=False)
