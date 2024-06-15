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
    public class CD_Usuarios
    {
        public List<Usuarios> Listar()
        {
            List<Usuarios> lista = new List<Usuarios>();
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    string query = "SELECT ID_USUARIO, NOMBRE, APELLIDOS, CORREO, CLAVE, REESTABLECER, ESTADO FROM usuario";
                    
                    MySqlCommand cmd = new MySqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader()){
                        while (reader.Read())
                        {
                            lista.Add(new Usuarios()
                            {
                                IdUsuario = Convert.ToInt32(reader["ID_USUARIO"]),
                                Nombre = reader["NOMBRE"].ToString(),
                                Apellidos = reader["APELLIDOS"].ToString(),
                                Correo = reader["CORREO"].ToString(),
                                Clave = reader["CLAVE"].ToString(),
                                Reestablecer = Convert.ToBoolean(reader["REESTABLECER"]),
                                Estado = Convert.ToBoolean(reader["ESTADO"])

                            }
                            );
                        }
                    }

                }
            }
            catch
            {
                lista = new List<Usuarios>();
            }

            return lista;
        }


        public int Registrar(Usuarios obj,out string Mensaje)
        {
            int idautogenerado = 0;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    MySqlCommand cmd = new MySqlCommand("sp_RegistrarUsuario", oconexion);
                    cmd.Parameters.AddWithValue("p_Nombres",obj.Nombre);
                    cmd.Parameters.AddWithValue("p_Apellidos",obj.Apellidos);
                    cmd.Parameters.AddWithValue("p_Correo",obj.Correo);
                    cmd.Parameters.AddWithValue("p_Clave",obj.Clave);
                    cmd.Parameters.AddWithValue("p_Estado",obj.Estado);
                    cmd.Parameters.Add("p_Resultado",MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_Mensaje", MySqlDbType.VarChar,500).Direction = ParameterDirection.Output;
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

        public bool Editar(Usuarios obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    MySqlCommand cmd = new MySqlCommand("sp_EditarUsuario", oconexion);
                    cmd.Parameters.AddWithValue("p_IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("p_Nombres", obj.Nombre);
                    cmd.Parameters.AddWithValue("p_Apellidos", obj.Apellidos);
                    cmd.Parameters.AddWithValue("p_Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("p_Clave", obj.Clave);
                    cmd.Parameters.AddWithValue("p_Estado", obj.Estado);
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
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM usuario WHERE Id_Usuario = @id ORDER BY Id_Usuario LIMIT 1;", oconexion);
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


        public bool CambiarClave(int idusuario,string nuevaclave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE usuario SET CLAVE = @nuevaclave, reestablecer = 0 WHERE Id_Usuario = @idusuario;", oconexion);
                    cmd.Parameters.AddWithValue("@idusuario", idusuario);
                    cmd.Parameters.AddWithValue("@nuevaclave", nuevaclave);
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



        public bool ReestablecerClave(int idusuario,string clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE usuario SET CLAVE = @clave, reestablecer = 1 WHERE Id_Usuario = @idusuario ORDER BY Id_Usuario LIMIT 1;", oconexion);
                    cmd.Parameters.AddWithValue("@id", idusuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
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
