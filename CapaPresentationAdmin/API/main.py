import pandas as pd
import json
import os
import sys
import mysql.connector
import shutil
from sqlalchemy import create_engine
from sqlalchemy.exc import SQLAlchemyError
import logging
from analysis_ventas import analisis_ventas
from analysis_association_rules import analisis_association_rules
from analysis_regression import analisis_regresion
from analysis_rfm import analisis_rfm
from analysis_time_series_clustering import analisis_time_series_clustering
from analysis_satisfaction_customer import calcular_satisfaccion
from analysis_risk_churn import analysis_risk_churn
from Recomendations import Generar_Recomendaciones

logging.basicConfig(level=logging.ERROR)
logger = logging.getLogger()

current_dir = os.path.dirname(os.path.abspath(__file__))
path_files = os.path.join(current_dir, 'files')
output_files = os.path.join(current_dir, 'output')
config_file_path = os.path.join(current_dir, 'Config.json')
ventas_limpias_file = os.path.join(current_dir, 'ventas_limpias.csv')

def clean_output_directory_and_file(output_dir, file_path):
    for archivo in os.listdir(output_dir):
        archivo_path = os.path.join(output_dir, archivo)
        try:
            if os.path.isfile(archivo_path) or os.path.islink(archivo_path):
                os.unlink(archivo_path)
            elif os.path.isdir(archivo_path):
                shutil.rmtree(archivo_path)
        except Exception as e:
            logger.error(f"Error al eliminar el archivo {archivo_path}: {e}")
    
    if os.path.exists(file_path):
        try:
            os.remove(file_path)
        except Exception as e:
            logger.error(f"Error al eliminar el archivo {file_path}: {e}")



def validation_files(path_files, path_output):
    lista_archivos = os.listdir(path_files)
    for archivo in lista_archivos:
        ubicacion_archivo2 = os.path.join(path_files, archivo)
        try:
            if 'CLIENTES' in archivo.upper():
                print("Archivo Clientes:")
                df = pd.read_excel(ubicacion_archivo2, dtype=str, skiprows=9)
                df.to_csv(os.path.join(path_output, 'clientes.csv'), sep='|', encoding='UTF-8', index=False, header=False)  
                print("Archivo CLIENTES generado\n")
            elif 'ARTICULO' in archivo.upper():
                print("Archivo Articulo:")
                df = pd.read_excel(ubicacion_archivo2, dtype=str, skiprows=10)
                df.to_csv(os.path.join(path_output, 'productos.csv'), sep='|', encoding='UTF-8', index=False, header=False)
                print("Archivo PRODUCTOS generado\n")
            elif 'VALORIZADO' in archivo.upper():
                print("Archivo Kardex Valorizado:")
                sheet_name = 'Detalle'
                df = pd.read_excel(ubicacion_archivo2, sheet_name=sheet_name, header=None, skiprows=8)
                records = []
                current_product_code = None
                for i in range(len(df)):
                    row = df.iloc[i]
                    if isinstance(row[0], str) and '|' in row[0]:
                        current_product_code = row[0].split('|')[0].strip()
                    elif isinstance(row[0], str) and row[0].startswith('FECHA'):
                        continue
                    elif row[0] != "SALDO INICIAL":
                        if not pd.isna(row[0]):
                            new_row = list(row)
                            new_row.insert(4, current_product_code)
                            records.append(new_row)
                columns = ['Fecha', 'Tipo de Documento', 'Serie', 'Número', 'Código Producto', 'Tipo de Operación', 'Entrada', 'Salida', 'Saldo']
                processed_df = pd.DataFrame(records, columns=columns)
                processed_df.to_csv(os.path.join(path_output, 'kardex_valorizado_erp.csv'), sep='|', encoding='UTF-8', index=False, header=False)
                print("Archivo Kardex Valorizado generado\n")
            elif 'VENTAS' in archivo.upper():
                print("Archivo Ventas ERP:")
                sheet_name = 'Detalle'
                df = pd.read_excel(ubicacion_archivo2, sheet_name=sheet_name, skiprows=9)
                df = df.applymap(lambda x: x.replace('\n', '') if isinstance(x, str) else x)
                df = df[df.iloc[:, 0] != 'TOTALIZADO:']
                df.to_csv(os.path.join(path_output, 'reporte_ventas_erp.csv'), sep='|', encoding='UTF-8', index=False, header=False)
                print("Archivo Ventas ERP generado\n")
            elif 'SEVEN' in archivo.upper():
                print("Archivo Ventas Input 1:")
                xls = pd.ExcelFile(ubicacion_archivo2)
                entrega_sheets = [sheet for sheet in xls.sheet_names if 'Entrega' in sheet and 'grafica' not in sheet.lower()]    
                for sheet in entrega_sheets:
                    df = pd.read_excel(xls, sheet_name=sheet, dtype=str)
                    df = df.applymap(lambda x: x.replace('\n', ' ') if isinstance(x, str) else x)
                    if 'figuras' in sheet.lower():
                        df = df.iloc[:, :22]
                        sheet2 = 'ventas_figuras'
                    elif 'consolas' in sheet.lower():
                        df = df.iloc[:, :21]
                        sheet2 = 'ventas_consolas'
                    elif 'perifericos' in sheet.lower():
                        df = df.iloc[:, :19]
                        sheet2 = 'ventas_perifericos'
                    csv_filename = os.path.join(path_output, f"{sheet2}.csv")
                    df.to_csv(csv_filename, sep='|', encoding='UTF-8', index=False, header=False)
                    print(f"Archivo CSV procesado: {csv_filename}")
            elif 'MOVE' in archivo.upper():
                print("Archivo Ventas Input 2:")
                xls = pd.ExcelFile(ubicacion_archivo2)
                entrega_sheets = [sheet for sheet in xls.sheet_names if 'Entrega' in sheet and 'grafica' not in sheet.lower()]    
                for sheet in entrega_sheets:
                    df = pd.read_excel(xls, sheet_name=sheet, dtype=str)
                    df = df.applymap(lambda x: x.replace('\n', ' ') if isinstance(x, str) else x)
                    df = df.iloc[:, :28]
                    csv_filename = os.path.join(path_output, "ventas_move.csv")
                    df.to_csv(csv_filename, sep='|', encoding='UTF-8', index=False, header=False)
                    print(f"Archivo CSV procesado: {csv_filename}")
            else:
                print('archivo inválido\n')
        except Exception as e3:
            logger.error(f"Error en el proceso de lectura de csv")
            logger.error(f"ERROR: {e3}")

def leer_configuracion(config_file_path):
    with open(config_file_path, 'r') as file:
        config = json.load(file)
    return config

def conectar_a_base_de_datos(config):
    try:
        connection = mysql.connector.connect(
            host=config.get('host'),
            database='bd_raw',
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

def main_db():
    config = leer_configuracion(config_file_path)
    connection = conectar_a_base_de_datos(config)
    if connection and connection.is_connected():
        output_dir = output_files
        for archivo in os.listdir(output_dir):
            if archivo.endswith('.csv'):
                table_name = os.path.splitext(archivo)[0]
                csv_file_path = os.path.join(output_dir, archivo)
                truncate_db_table(connection, table_name)
                insert_data(connection, csv_file_path, table_name)
        connection.close()
        print("Conexión cerrada")


def ejecutar_procedimiento(engine, procedimiento):
    try:
        connection = engine.raw_connection()
        cursor = connection.cursor()
        cursor.callproc(procedimiento)
        connection.commit()
        cursor.close()
        connection.close()
        print(f"Procedimiento {procedimiento} ejecutado con éxito")
    except SQLAlchemyError as e:
        print(f"Error al ejecutar el procedimiento {procedimiento}: {e}")

def bd_process():
    with open(config_file_path, 'r') as file:
        creds = json.load(file)

    conn_str_raw = f"mysql+pymysql://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/bd_raw"
    engine_raw = create_engine(conn_str_raw)
    ejecutar_procedimiento(engine_raw, 'Raw_to_control')

    conn_str_control = f"mysql+pymysql://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/bd_control"
    engine_control = create_engine(conn_str_control)
    ejecutar_procedimiento(engine_control, 'Control_to_Olap')


def DataMining_Fuction():
    with open(config_file_path, 'r') as file:
        creds = json.load(file)

    conn_str = f"mysql+mysqldb://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
    engine = create_engine(conn_str)

    try:
        data = pd.read_sql('SELECT * FROM ventas', engine)
    except SQLAlchemyError as e:
        print(f"Error al conectar a la base de datos: {e}")
        return

    data['FECHA'] = pd.to_datetime(data['FECHA'], errors='coerce')
    data = data.dropna(subset=['FECHA'])
    data.fillna(0, inplace=True)
    data.to_csv(ventas_limpias_file, index=False)

    analisis_ventas(ventas_limpias_file)
    analisis_association_rules(ventas_limpias_file)
    analisis_regresion(ventas_limpias_file)
    analisis_rfm(ventas_limpias_file)
    analisis_time_series_clustering(ventas_limpias_file)
    calcular_satisfaccion(ventas_limpias_file)
    analysis_risk_churn(ventas_limpias_file)


def main():
    clean_output_directory_and_file(output_files, ventas_limpias_file)
    validation_files(path_files, output_files)
    main_db()
    bd_process()
    DataMining_Fuction()
    Generar_Recomendaciones(ventas_limpias_file)

if __name__ == "__main__":
    main()
