import pandas as pd
import json
import os
from sqlalchemy import create_engine
from sklearn.model_selection import train_test_split
from sklearn.linear_model import LinearRegression

script_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, 'Config.json')
with open(config_path, 'r') as file:
    creds = json.load(file)

conn_str = f"mysql+pymysql://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
engine = create_engine(conn_str)

def analisis_regresion(file_path):
    data = pd.read_csv(file_path)
    data['FECHA'] = pd.to_datetime(data['FECHA'], errors='coerce')
    data['MES'] = data['FECHA'].dt.month
    data['AÑO'] = data['FECHA'].dt.year

    features = data[['MES', 'AÑO', 'NOMBRE_CLIENTE', 'FECHA']].dropna()
    target = data.loc[features.index, 'VALORSINIGV']

    X_train, X_test, y_train, y_test = train_test_split(features[['MES', 'AÑO']], target, test_size=0.2, random_state=42)

    X_train = X_train.fillna(0)
    y_train = y_train.fillna(0)

    model = LinearRegression()
    model.fit(X_train, y_train)

    predictions = model.predict(X_test)
    predictions_df = pd.DataFrame({
        'NOMBRE_CLIENTE': features.loc[X_test.index, 'NOMBRE_CLIENTE'],
        'FECHA': features.loc[X_test.index, 'FECHA'],
        'PREDICTED': predictions,
        'ACTUAL': y_test
    })
    
    predictions_df.to_sql('predicciones_ventas', engine, if_exists='replace', index=False)
