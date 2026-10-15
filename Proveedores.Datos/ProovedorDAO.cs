using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    // CRUD de la tabla PROVEEDOR. Todo el SQL de esta entidad vive aquí.
    public class ProveedorDAO
    {
        // Para mostrar en tablas: une proveedor con ciudad.
        public List<ProveedorVistaDto> ListarVista()
        {
            var lista = new List<ProveedorVistaDto>();
            const string sql =
                "SELECT pr.cedula, pr.nombre || ' ' || pr.apellido AS nombre_completo, c.nombre AS ciudad " +
                "FROM proveedor pr " +
                "JOIN ciudad c ON c.id = pr.ciudad_id " +
                "ORDER BY pr.apellido, pr.nombre";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    con.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new ProveedorVistaDto
                            {
                                Cedula = rd.GetString(0),
                                NombreCompleto = rd.GetString(1),
                                Ciudad = rd.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }

            return lista;
        }

        public List<Proveedor> Listar()
        {
            var lista = new List<Proveedor>();
            const string sql =
                "SELECT cedula, nombre, apellido, fecha_nacimiento, ciudad_id " +
                "FROM proveedor ORDER BY apellido, nombre";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    con.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(Mapear(rd));
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }

            return lista;
        }

        public Proveedor? ObtenerPorCedula(string cedula)
        {
            const string sql =
                "SELECT cedula, nombre, apellido, fecha_nacimiento, ciudad_id " +
                "FROM proveedor WHERE cedula = :cedula";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;
                    con.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            return Mapear(rd);
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }

            return null;
        }

        public int Insertar(Proveedor proveedor)
        {
            const string sql =
                "INSERT INTO proveedor (cedula, nombre, apellido, fecha_nacimiento, ciudad_id) " +
                "VALUES (:cedula, :nombre, :apellido, :fecha_nacimiento, :ciudad_id)";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = proveedor.Cedula;
                    cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = proveedor.Nombre;
                    cmd.Parameters.Add("apellido", OracleDbType.Varchar2).Value = proveedor.Apellido;
                    cmd.Parameters.Add("fecha_nacimiento", OracleDbType.Date).Value = proveedor.FechaNacimiento;
                    cmd.Parameters.Add("ciudad_id", OracleDbType.Int32).Value = proveedor.CiudadId;
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

        // La cédula es la llave: no se actualiza, solo se usa para ubicar la fila.
        public int Actualizar(Proveedor proveedor)
        {
            const string sql =
                "UPDATE proveedor SET nombre = :nombre, apellido = :apellido, " +
                "fecha_nacimiento = :fecha_nacimiento, ciudad_id = :ciudad_id " +
                "WHERE cedula = :cedula";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = proveedor.Nombre;
                    cmd.Parameters.Add("apellido", OracleDbType.Varchar2).Value = proveedor.Apellido;
                    cmd.Parameters.Add("fecha_nacimiento", OracleDbType.Date).Value = proveedor.FechaNacimiento;
                    cmd.Parameters.Add("ciudad_id", OracleDbType.Int32).Value = proveedor.CiudadId;
                    cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = proveedor.Cedula;
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

        public int Eliminar(string cedula)
        {
            const string sql = "DELETE FROM proveedor WHERE cedula = :cedula";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

        private static Proveedor Mapear(OracleDataReader rd)
        {
            return new Proveedor
            {
                Cedula = rd.GetString(0),
                Nombre = rd.GetString(1),
                Apellido = rd.GetString(2),
                FechaNacimiento = rd.GetDateTime(3),
                CiudadId = rd.GetInt32(4)
            };
        }
    }
}