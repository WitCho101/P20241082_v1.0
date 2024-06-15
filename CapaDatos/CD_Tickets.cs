using CapaEntidad;
using MySql.Data.MySqlClient;
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
    public class CD_Tickets
    {
        public List<Ticket> Listar()
        {
            List<Ticket> lista = new List<Ticket>();
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    string query = "SELECT T.ID_TICKET, U.ID_USUARIO, U.NOMBRE, U.CORREO, T.ASUNTO, T.PRIORIDAD, T.ESTADO FROM ticket AS T INNER JOIN usuario AS U ON T.ID_USUARIO = U.ID_USUARIO";

                    MySqlCommand cmd = new MySqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Ticket()
                            {
                                IdTicket = Convert.ToInt32(reader["ID_TICKET"]),
                                oUsuario = new Usuarios() { IdUsuario = Convert.ToInt32(reader["ID_USUARIO"]), Nombre = reader["NOMBRE"].ToString(), Correo = reader["CORREO"].ToString() },
                                Asunto = reader["ASUNTO"].ToString(),
                                Prioridad = Convert.ToBoolean(reader["PRIORIDAD"]),
                                Estado = Convert.ToBoolean(reader["ESTADO"])
                            }
                            );
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Ticket>();
            }

            return lista;
        }


        
        public int Registrar(Ticket obj, out string Mensaje)
        {
            int idautogenerado = 0;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    MySqlCommand cmd = new MySqlCommand("sp_RegistrarTicket", oconexion);
                    cmd.Parameters.AddWithValue("p_Id_Usuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("p_NombreUsuario", obj.oUsuario.Nombre);
                    cmd.Parameters.AddWithValue("p_CorreoUsuario", obj.oUsuario.Correo);
                    cmd.Parameters.AddWithValue("p_ASUNTO", obj.Asunto);
                    cmd.Parameters.AddWithValue("p_PRIORIDAD", obj.Prioridad);
                    cmd.Parameters.AddWithValue("p_ESTADO", obj.Estado);
                    cmd.Parameters.Add("p_Resultado", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_Mensaje", MySqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(cmd.Parameters["p_Resultado"].Value);
                    Mensaje = cmd.Parameters["p_Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;
        }



        public bool Editar(Ticket obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    MySqlCommand cmd = new MySqlCommand("sp_EditarTicket", oconexion);
                    cmd.Parameters.AddWithValue("p_Id_Ticket", obj.IdTicket);
                    cmd.Parameters.AddWithValue("p_Id_Usuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("p_NombreUsuario", obj.oUsuario.Nombre);
                    cmd.Parameters.AddWithValue("p_CorreoUsuario", obj.oUsuario.Correo);
                    cmd.Parameters.AddWithValue("p_ASUNTO", obj.Asunto);
                    cmd.Parameters.AddWithValue("p_PRIORIDAD", obj.Prioridad);
                    cmd.Parameters.AddWithValue("p_ESTADO", obj.Estado);
                    cmd.Parameters.Add("p_Resultado", MySqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_Mensaje", MySqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["p_Resultado"].Value);
                    Mensaje = cmd.Parameters["p_Mensaje"].Value.ToString();

                }

            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;

        }


        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM ticket WHERE Id_Ticket = @id ORDER BY Id_Ticket LIMIT 1;", oconexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;

                }

            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;

        }


    }
}
