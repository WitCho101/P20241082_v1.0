import pandas as pd
import numpy as np
import os
from datetime import datetime, timedelta
from sqlalchemy import create_engine
import json

def Generar_Recomendaciones(file_path):

    script_dir = os.path.dirname(os.path.abspath(__file__))
    config_path = os.path.join(script_dir, 'Config.json')
    with open(config_path, 'r') as file:
        creds = json.load(file)

    conn_str = f"mysql+mysqldb://{creds['username']}:{creds['password']}@{creds['host']}:{creds['port']}/{creds['database']}"
    engine = create_engine(conn_str)

    data = pd.read_csv(file_path)

    data['FECHA'] = pd.to_datetime(data['FECHA'])

    current_date = datetime.now()
    rfm = data.groupby('NOMBRE_CLIENTE').agg({
        'FECHA': lambda x: (current_date - x.max()).days,
        'NRO_DOCUMENTO': 'count',
        'VALORSINIGV': 'sum'
    }).reset_index()

    rfm.columns = ['NOMBRE_CLIENTE', 'Recencia', 'Frecuencia', 'ValorMonetario']

    conditions = [
        (rfm['Recencia'] <= 30) & (rfm['Frecuencia'] >= 5),
        (rfm['Recencia'] <= 30) & (rfm['Frecuencia'] < 5),
        (rfm['Recencia'] > 30) & (rfm['Frecuencia'] >= 5),
        (rfm['Recencia'] > 30) & (rfm['Frecuencia'] < 5)
    ]
    choices = ['Cliente Leal', 'Cliente Nuevo', 'Cliente en Riesgo', 'Cliente Perdido']
    rfm['Segmento'] = np.select(conditions, choices, default='Cliente General')

    def generar_recomendaciones(segmento):
        if segmento == 'Cliente Leal':
            return 'Ofrecer descuentos exclusivos y programas de lealtad para mantener la fidelidad del cliente.'
        elif segmento == 'Cliente Nuevo':
            return 'Enviar una bienvenida y ofrecer productos complementarios para fomentar mas compras.'
        elif segmento == 'Cliente en Riesgo':
            return 'Enviar recordatorios y ofrecer descuentos para reenganchar al cliente antes de que se pierda.'
        elif segmento == 'Cliente Perdido':
            return 'Enviar encuestas para entender la razon de la perdida y ofrecer incentivos atractivos para que regrese.'
        else:
            return 'Mantener comunicacion regular y ofrecer recomendaciones personalizadas basadas en compras anteriores.'

    rfm['Recomendaciones'] = rfm['Segmento'].apply(generar_recomendaciones)

    productos_vendidos = data.groupby('DESCRIPCION_PRODUCTO').agg({
        'VALORSINIGV': 'sum',
        'NRO_DOCUMENTO': 'count'
    }).reset_index()

    productos_vendidos.columns = ['DESCRIPCION_PRODUCTO', 'Total_Ventas', 'Cantidad_Vendida']
    productos_vendidos = productos_vendidos.sort_values(by='Cantidad_Vendida', ascending=False)

    data['Mes'] = data['FECHA'].dt.to_period('M')
    ventas_por_mes = data.groupby(['Mes', 'DESCRIPCION_PRODUCTO']).agg({
        'VALORSINIGV': 'sum',
        'NRO_DOCUMENTO': 'count'
    }).reset_index()

    ventas_por_mes.columns = ['Mes', 'DESCRIPCION_PRODUCTO', 'Total_Ventas', 'Cantidad_Vendida']

    ventas_por_mes = ventas_por_mes[ventas_por_mes['Cantidad_Vendida'] > 14]

    def generar_recomendaciones_campanas(producto, mes, cantidad):
        return f"En {mes}, {producto} tuvo una alta demanda con {cantidad} unidades vendidas. Se recomienda lanzar un envento promocional para este producto en fechas similares."

    ventas_por_mes['Recomendaciones_Campana'] = ventas_por_mes.apply(
        lambda x: generar_recomendaciones_campanas(x['DESCRIPCION_PRODUCTO'], x['Mes'], x['Cantidad_Vendida']),
        axis=1
    )

    ultimos_dos_meses = current_date - timedelta(days=60)
    productos_recientes = data[data['FECHA'] >= ultimos_dos_meses]
    productos_recientes_agrupados = productos_recientes.groupby(['Mes', 'DESCRIPCION_PRODUCTO']).agg({
        'VALORSINIGV': 'sum',
        'NRO_DOCUMENTO': 'count'
    }).reset_index()

    productos_recientes_agrupados.columns = ['Mes', 'DESCRIPCION_PRODUCTO', 'Total_Ventas', 'Cantidad_Vendida']
    productos_recientes_agrupados = productos_recientes_agrupados[productos_recientes_agrupados['Cantidad_Vendida'] > 14]

    def generar_recomendaciones_adicionales(mes, producto, cantidad):
        return f"En el mes {mes}, el producto {producto} tuvo una alta demanda con {cantidad} unidades vendidas. Considera futuras promociones para este producto."

    productos_recientes_agrupados['Recomendaciones_Adicionales'] = productos_recientes_agrupados.apply(
        lambda x: generar_recomendaciones_adicionales(x['Mes'], x['DESCRIPCION_PRODUCTO'], x['Cantidad_Vendida']),
        axis=1
    )

    def generar_recomendaciones_usuario_producto(usuario, mes, cantidad, total_ventas, cantidad_clientes):
        if cantidad > 100:
            return f"En el mes {mes}, el usuario {usuario} ha vendido {cantidad} unidades de productos, obteniendo una suma de S/{total_ventas:.2f} de {cantidad_clientes} clientes. Se recomienda incentivar al usuario con bonos."
        elif 50 <= cantidad <= 100:
            return f"En el mes {mes}, el usuario {usuario} ha vendido {cantidad} unidades de productos, obteniendo una suma de S/{total_ventas:.2f} de {cantidad_clientes} clientes. Considera promociones especificas para este usuario."
        else:
            return f"En el mes {mes}, el usuario {usuario} ha vendido {cantidad} unidades de productos, obteniendo una suma de S/{total_ventas:.2f} de {cantidad_clientes} clientes. Se sugiere monitorear las ventas para posibles incentivos futuros."

    recomendaciones_usuario_producto = data.groupby(['USUARIO', 'Mes']).agg({
        'CODIGO_PRODUCTO': 'count',
        'VALORSINIGV': 'sum',
        'NOMBRE_CLIENTE': 'nunique'
    }).reset_index()

    recomendaciones_usuario_producto.columns = ['USUARIO', 'Mes', 'Cantidad_Producto', 'Total_Ventas', 'Cantidad_Clientes']
    recomendaciones_usuario_producto = recomendaciones_usuario_producto.sort_values(by='Cantidad_Producto', ascending=False)
    recomendaciones_usuario_producto['Recomendaciones_Usuario_Producto'] = recomendaciones_usuario_producto.apply(
        lambda x: generar_recomendaciones_usuario_producto(x['USUARIO'], x['Mes'], x['Cantidad_Producto'], x['Total_Ventas'], x['Cantidad_Clientes']),
        axis=1
    )

    def generar_recomendaciones_usuario_cliente(usuario, mes, cantidad_clientes, total_ventas):
        if cantidad_clientes > 150:
            return f"En el mes {mes}, el usuario {usuario} ha vendido a {cantidad_clientes} clientes, obteniendo una suma de S/{total_ventas:.2f}. Se recomienda brindar un bono al usuario por el gran impacto en la empresa."
        elif 100 <= cantidad_clientes <= 150:
            return f"En el mes {mes}, el usuario {usuario} ha vendido a {cantidad_clientes} clientes, obteniendo una suma de S/{total_ventas:.2f}. Se sugiere considerar promociones especificas para este usuario."
        else:
            return f"En el mes {mes}, el usuario {usuario} ha vendido a {cantidad_clientes} clientes, obteniendo una suma de S/{total_ventas:.2f}. Se recomienda monitorear las ventas para posibles incentivos futuros."

    recomendaciones_usuario_cliente = data.groupby(['USUARIO', 'Mes']).agg({
        'NOMBRE_CLIENTE': 'nunique',
        'VALORSINIGV': 'sum'
    }).reset_index()

    recomendaciones_usuario_cliente.columns = ['USUARIO', 'Mes', 'Cantidad_Clientes', 'Total_Ventas']
    recomendaciones_usuario_cliente = recomendaciones_usuario_cliente.sort_values(by='Cantidad_Clientes', ascending=False)
    recomendaciones_usuario_cliente['Recomendaciones_Usuario_Cliente'] = recomendaciones_usuario_cliente.apply(
        lambda x: generar_recomendaciones_usuario_cliente(x['USUARIO'], x['Mes'], x['Cantidad_Clientes'], x['Total_Ventas']),
        axis=1
    )

    rfm.to_sql('recomendaciones_clientes', engine, if_exists='replace', index=False)
    ventas_por_mes[['Mes', 'DESCRIPCION_PRODUCTO', 'Recomendaciones_Campana']].to_sql('recomendaciones_campanas', engine, if_exists='replace', index=False)
    productos_recientes_agrupados[['Mes', 'DESCRIPCION_PRODUCTO', 'Recomendaciones_Adicionales']].to_sql('recomendaciones_adicionales', engine, if_exists='replace', index=False)
    recomendaciones_usuario_producto[['USUARIO', 'Mes', 'Cantidad_Producto', 'Total_Ventas', 'Cantidad_Clientes', 'Recomendaciones_Usuario_Producto']].to_sql('recomendaciones_usuario_producto', engine, if_exists='replace', index=False)
    recomendaciones_usuario_cliente[['USUARIO', 'Mes', 'Cantidad_Clientes', 'Total_Ventas', 'Recomendaciones_Usuario_Cliente']].to_sql('recomendaciones_usuario_cliente', engine, if_exists='replace', index=False)

