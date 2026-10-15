using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    // Escrituras de la tabla BITACORA. En CU-01 solo se registra la
    // entrada; la salida (fecha_salida) es del CU-02.
    public class BitacoraDAO
    {
        // Excepcion al patron: devuelve el id generado por la identidad.
        public int RegistrarEntrada(string cedula)
        {
            const string sql =
                "INSERT INTO bitacora (usuario_cedula) VALUES (:cedula) " +
                "RETURNING id INTO :id";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;

                    // El id lo devuelve la base, en un parametro de salida.
                    var parametroId = new OracleParameter("id", OracleDbType.Int32);
                    parametroId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(parametroId);

                    con.Open();
                    using (var trans = con.BeginTransaction())
                    {
                        cmd.Transaction = trans;
                        cmd.ExecuteNonQuery();
                        trans.Commit();   // la fila debe quedar confirmada
                    }

                    // En el driver administrado el valor llega como OracleDecimal.
                    return Convert.ToInt32(parametroId.Value.ToString());
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

    public void RegistrarSalida(int bitacoraId)
        {
            try
            {
                using var con = Conexion.Crear();
                con.Open();

                using var cmd = con.CreateCommand();
                cmd.BindByName = true;
                cmd.CommandText =
                    @"UPDATE bitacora
                 SET fecha_salida = SYSTIMESTAMP
               WHERE id = :id
                 AND fecha_salida IS NULL";
                cmd.Parameters.Add(new OracleParameter("id", bitacoraId));

                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

        public List<BitacoraVistaDto> ListarTodos(DateTime desde, DateTime hasta)
        {
            const string sql = @"
                SELECT u.nombre || ' ' || u.apellido AS usuario,
                       b.fecha_entrada,
                       b.fecha_salida
                FROM bitacora b
                JOIN usuario u ON u.cedula = b.usuario_cedula
                WHERE b.fecha_entrada >= :desde
                  AND b.fecha_entrada <  :hasta
                ORDER BY b.fecha_entrada DESC";

            return EjecutarConsulta(sql, desde, hasta);
        }

        public List<BitacoraVistaDto> ListarPorUsuario(string cedulaUsuario, DateTime desde, DateTime hasta)
        {
            const string sql = @"
                SELECT u.nombre || ' ' || u.apellido AS usuario,
                       b.fecha_entrada,
                       b.fecha_salida
                FROM bitacora b
                JOIN usuario u ON u.cedula = b.usuario_cedula
                WHERE b.usuario_cedula = :cedula
                  AND b.fecha_entrada >= :desde
                  AND b.fecha_entrada <  :hasta
                ORDER BY b.fecha_entrada DESC";

            return EjecutarConsulta(sql, desde, hasta, cedulaUsuario);
        }

        private static List<BitacoraVistaDto> EjecutarConsulta(
            string sql, DateTime desde, DateTime hasta, string? cedulaUsuario = null)
        {
            try
            {
                using var con = Conexion.Crear();
                con.Open();

                using var cmd = new OracleCommand(sql, con) { BindByName = true };
                if (cedulaUsuario != null)
                    cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedulaUsuario;
                cmd.Parameters.Add("desde", OracleDbType.TimeStamp).Value = desde.Date;
                cmd.Parameters.Add("hasta", OracleDbType.TimeStamp).Value = hasta.Date.AddDays(1);

                var lista = new List<BitacoraVistaDto>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new BitacoraVistaDto
                    {
                        Usuario = reader.GetString(0),
                        FechaEntrada = reader.GetDateTime(1),
                        FechaSalida = reader.IsDBNull(2) ? null : reader.GetDateTime(2)
                    });
                }
                return lista;
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }
    }
}

