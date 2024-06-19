import pandas as pd
import json
import os
from sqlalchemy import create_engine
from mlxtend.frequent_patterns import apriori, association_rules

script_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, 'Config.json')
with open(config_path, 'r') as file:
    creds = json.load(file)

conn_str = f"mysql+mysqldb://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
engine = create_engine(conn_str)

def analisis_association_rules(file_path):
    data = pd.read_csv(file_path)
    basket = data.groupby(['NOMBRE_CLIENTE', 'DESCRIPCION_PRODUCTO'])['NRO_DOCUMENTO'].count().unstack().reset_index().fillna(0).set_index('NOMBRE_CLIENTE')
    basket = basket.applymap(lambda x: 1 if x > 0 else 0)
    frequent_itemsets = apriori(basket.astype(bool), min_support=0.01, use_colnames=True)
    rules = association_rules(frequent_itemsets, metric="lift", min_threshold=1)
    rules['antecedents'] = rules['antecedents'].apply(lambda x: ', '.join(list(x)))
    rules['consequents'] = rules['consequents'].apply(lambda x: ', '.join(list(x)))

    rules.to_sql('reglas_asociacion', engine, if_exists='replace', index=False)