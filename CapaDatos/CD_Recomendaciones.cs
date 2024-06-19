using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaEntidad;
using MySql.Data.MySqlClient;

namespace CapaDatos
{
    public class CD_Recomendaciones
    {

        public List<Rdcn_Cliente> Listar_Recomendaciones_Clientes()
        {
            List<Rdcn_Cliente> lista = new List<Rdcn_Cliente>();
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM recomendaciones_clientes where nombre_cliente != '-'";

                    MySqlCommand cmd = new MySqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Rdcn_Cliente()
                            {
                                Nombre_cliente = reader["NOMBRE_CLIENTE"].ToString(),
                                Recencia = Convert.ToInt32(reader["Recencia"]),
                                Frecuencia = Convert.ToInt32(reader["Frecuencia"]),
                                Valor_monetario = Convert.ToDecimal(reader["ValorMonetario"]),
                                Segmento = reader["Segmento"].ToString(),
                                Recomendacion_cliente = reader["Recomendaciones"].ToString()

                            }
                            );
                        }
                    }

                }
            }
            catch
            {
                lista = new List<Rdcn_Cliente>();
            }

            return lista;
        }


        public List<Rdcn_campanas> Listar_Recomendaciones_Campanas()
        {
            List<Rdcn_campanas> lista = new List<Rdcn_campanas>();
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM recomendaciones_campanas WHERE CAST(SUBSTRING(mes, 1, 4) AS UNSIGNED) >= 2021 order by mes desc";

                    MySqlCommand cmd = new MySqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Rdcn_campanas()
                            {
                                Mes = reader["Mes"].ToString(),
                                Descripcion_producto = reader["Descripcion_producto"].ToString(),
                                Recomendacion_campana = reader["Recomendaciones_campana"].ToString()
                            }
                            );
                        }
                    }

                }
            }
            catch
            {
                lista = new List<Rdcn_campanas>();
            }

            return lista;
        }



        public List<Rdcn_adicionales> Listar_Recomendaciones_Adicionales()
        {
            List<Rdcn_adicionales> lista = new List<Rdcn_adicionales>();
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM recomendaciones_adicionales order by mes desc";

                    MySqlCommand cmd = new MySqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Rdcn_adicionales()
                            {
                                Mes = reader["Mes"].ToString(),
                                Descripcion_producto = reader["Descripcion_producto"].ToString(),
                                Recomendacion_adicional = reader["Recomendaciones_Adicionales"].ToString()
                            }
                            );
                        }
                    }

                }
            }
            catch
            {
                lista = new List<Rdcn_adicionales>();
            }

            return lista;
        }

        public List<Rdcn_usuario_cliente> Listar_Recomendaciones_Usuario_Cliente()
        {
            List<Rdcn_usuario_cliente> lista = new List<Rdcn_usuario_cliente>();
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM recomendaciones_usuario_cliente WHERE usuario NOT REGEXP '[0-9]' AND usuario NOT IN ('','-') AND CAST(SUBSTRING(mes, 1, 4) AS UNSIGNED) >= 2021 ORDER BY mes DESC";

                    MySqlCommand cmd = new MySqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Rdcn_usuario_cliente()
                            {
                                Usuario = reader["Usuario"].ToString(),
                                Mes = reader["Mes"].ToString(),
                                Cantidad_cliente = Convert.ToInt32(reader["Cantidad_clientes"]),
                                Total_ventas = Convert.ToDecimal(reader["Total_ventas"]),
                                Recomendacion_usuario_cliente = reader["Recomendaciones_Usuario_Cliente"].ToString()
                            }
                            );
                        }
                    }

                }
            }
            catch
            {
                lista = new List<Rdcn_usuario_cliente>();
            }

            return lista;
        }


        public List<Rdcn_usuario_producto> Listar_Recomendaciones_Usuario_Productos()
        {
            List<Rdcn_usuario_producto> lista = new List<Rdcn_usuario_producto>();
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM recomendaciones_usuario_producto WHERE usuario NOT REGEXP '[0-9]' AND usuario NOT IN ('','-') AND CAST(SUBSTRING(mes, 1, 4) AS UNSIGNED) >= 2021 ORDER BY mes DESC";

                    MySqlCommand cmd = new MySqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Rdcn_usuario_producto()
                            {
                                Usuario = reader["Usuario"].ToString(),
                                Mes = reader["Mes"].ToString(),
                                Cantidad_producto = Convert.ToInt32(reader["Cantidad_Producto"]),
                                Total_ventas = Convert.ToDecimal(reader["Total_ventas"]),
                                Cantidad_cliente = Convert.ToInt32(reader["Cantidad_clientes"]),
                                Recomendacion_usuario_producto = reader["Recomendaciones_Usuario_Producto"].ToString()
                            }
                            );
                        }
                    }

                }
            }
            catch
            {
                lista = new List<Rdcn_usuario_producto>();
            }

            return lista;
        }


    }
}
