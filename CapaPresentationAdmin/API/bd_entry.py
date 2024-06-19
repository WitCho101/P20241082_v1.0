import mysql.connector
import pandas as pd
import os
import sys
import json

def leer_configuracion(filename='Config.json'):
    with open(filename, 'r') as file:
        config = json.load(file)
    return config

def conectar_a_base_de_datos(config):
    try:
        connection = mysql.connector.connect(
            host=config.get('host'),
            database=config.get('database'),
            user=config.get('username'),
            password=config.get('password'),
            port=config.get('port')
        )
        if connection.is_connected():
            print("Conexión exitosa a la base de datos")
            return connection
    except mysql.connector.Error as e:
        print(f"Error al conectar a MySQL: {e}")
        return None

def truncate_db_table(connection, table_name):
    try:
        cursor = connection.cursor()
        query = f"TRUNCATE TABLE {table_name}"
        cursor.execute(query)
        connection.commit()
        print(f"Tabla {table_name} truncada con éxito")
    except mysql.connector.Error as e:
        print(f"Error al truncar la tabla {table_name}: {e}")

def insert_data(connection, csv_file_path, table_name):
    try:
        df = pd.read_csv(csv_file_path, delimiter='|', dtype=str, header=None).fillna('')
        cursor = connection.cursor()
        values_placeholder = ", ".join(["%s"] * len(df.columns))
        insert_query = f"INSERT INTO `{table_name}` VALUES ({values_placeholder})"
        data = [tuple(row) for row in df.values]
        cursor.executemany(insert_query, data)
        connection.commit()
        print(f"Datos insertados en la tabla {table_name}")
    except Exception as e:
        print(f"Unexpected error en func 'insert_data': {e}, {sys.exc_info()}")

def main():
    config = leer_configuracion()
    connection = conectar_a_base_de_datos(config)
    if connection and connection.is_connected():
        output_dir = 'output'
        for archivo in os.listdir(output_dir):
            if archivo.endswith('.csv'):
                table_name = os.path.splitext(archivo)[0]
                csv_file_path = os.path.join(output_dir, archivo)
                
                truncate_db_table(connection, table_name)
                insert_data(connection, csv_file_path, table_name)
        
        connection.close()
        print("Conexión cerrada")

if __name__ == "__main__":
    main()
