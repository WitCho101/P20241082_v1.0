import os
import pyodbc
import mysql.connector
from mysql.connector import Error
import pandas as pd
import glob
from os.path import join
import logging
logging.basicConfig(level=logging.ERROR)
logger = logging.getLogger()

# path_files = './files'
# output_files = './output'

def validation_files (path_files,path_output):
    lista_archivos = os.listdir(path_files)
    for archivo in lista_archivos:
        ubicacion_archivo2 = join(path_files,archivo)
        try:
            if (archivo.upper().__contains__('CLIENTES')):
                print("Archivo Clientes:")
                
                df = pd.read_excel(ubicacion_archivo2,dtype=str,skiprows=9)
                df.to_csv('./output/CLIENTES.csv', sep='|', encoding='UTF-8', index=False, header= False)  
                print("Archivo CLIENTES generado\n")

            elif (archivo.upper().__contains__('ARTICULO')):
                print("Archivo Articulo:")
                
                df = pd.read_excel(ubicacion_archivo2,dtype=str,skiprows=10)
                df.to_csv('./output/PRODUCTOS.csv', sep='|', encoding='UTF-8', index=False, header= False)
                print("Archivo PRODUCTOS generado\n")
            
            elif (archivo.upper().__contains__('VALORIZADO')):
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
                processed_df.to_csv('./output/KARDEX_VALORIZADO_ERP.csv', sep='|', encoding='UTF-8', index=False, header= False)
                print("Archivo Kardex Valorizado generado\n")
            elif (archivo.upper().__contains__('VENTAS')):
                print("Archivo Ventas ERP:")
                
                sheet_name = 'Detalle'
                df = pd.read_excel(ubicacion_archivo2, sheet_name=sheet_name, skiprows=9)
                df = df.applymap(lambda x: x.replace('\n', '') if isinstance(x, str) else x)
                df = df[df.iloc[:, 0] != 'TOTALIZADO:']
                df.to_csv('./output/REPORTE_VENTAS_ERP.csv', sep='|', encoding='UTF-8', index=False, header= False)
                print("Archivo Ventas ERP generado\n")
            elif (archivo.upper().__contains__('SEVEN')):
                print("Archivo Ventas Input 1:")
                xls = pd.ExcelFile(ubicacion_archivo2)
                entrega_sheets = [sheet for sheet in xls.sheet_names if 'Entrega' in sheet and 'grafica' not in sheet.lower()]    
                for sheet in entrega_sheets:
                    df = pd.read_excel(xls, sheet_name=sheet, dtype=str)
                    df = df.applymap(lambda x: x.replace('\n', ' ') if isinstance(x, str) else x)
                    if 'figuras' in sheet.lower():
                        df = df.iloc[:, :22]
                        sheet2 = 'VENTAS_FIGURAS'
                    elif 'consolas' in sheet.lower():
                        df = df.iloc[:, :21]
                        sheet2 = 'VENTAS_CONSOLAS'
                    elif 'perifericos' in sheet.lower():
                        df = df.iloc[:, :19]
                        sheet2 = 'VENTAS_PERIFERICOS'
                    
                    csv_filename = os.path.join(path_output, f"{sheet2}.csv")
        
                    df.to_csv(csv_filename, sep='|', encoding='UTF-8', index=False, header=False)
                    print(f"Archivo CSV procesado: {csv_filename}")
            elif (archivo.upper().__contains__('MOVE')):
                print("Archivo Ventas Input 2:")
                xls = pd.ExcelFile(ubicacion_archivo2)
                entrega_sheets = [sheet for sheet in xls.sheet_names if 'Entrega' in sheet and 'grafica' not in sheet.lower()]    
                for sheet in entrega_sheets:
                    df = pd.read_excel(xls, sheet_name=sheet, dtype=str)
                    df = df.applymap(lambda x: x.replace('\n', ' ') if isinstance(x, str) else x)
                    df = df.iloc[:, :28]
                    csv_filename = os.path.join(path_output, "VENTAS_MOVE.csv")
        
                    df.to_csv(csv_filename, sep='|', encoding='UTF-8', index=False, header=False)
                    print(f"Archivo CSV procesado: {csv_filename}")

            else:
                print('archivo inválido\n')
        except Exception as e3:
            logger.error(f"Error en el porceso de lectura de csv")
            logger.error(f"ERROR: {e3}")
