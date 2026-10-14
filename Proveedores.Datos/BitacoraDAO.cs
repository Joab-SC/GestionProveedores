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
    }
}
