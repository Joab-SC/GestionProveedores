using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Proveedores.Datos;

public class DetalleProductoDAO
{
    public List<int> ListarProductoIdsDeProveedor(string cedula)
    {
        var ids = new List<int>();
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.BindByName = true;
            cmd.CommandText =
                "SELECT producto_id FROM detalle_producto WHERE proveedor_cedula = :cedula";
            cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                ids.Add(rd.GetInt32(0));
        }
        catch (OracleException ex)
        {
            throw TraductorErroresOracle.Traducir(ex);
        }
        return ids;
    }

    public void ReemplazarProductosDeProveedor(string cedulaProveedor, List<int> productoIds)
    {
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var tx = con.BeginTransaction();
            try
            {
                // 1) Borrar todas las asignaciones actuales del proveedor
                using (var del = con.CreateCommand())
                {
                    del.Transaction = tx;
                    del.BindByName = true;
                    del.CommandText =
                        "DELETE FROM detalle_producto WHERE proveedor_cedula = :cedula";
                    del.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedulaProveedor;
                    del.ExecuteNonQuery();
                }

                // 2) Insertar las nuevas, una por una
                foreach (var productoId in productoIds)
                {
                    using var ins = con.CreateCommand();
                    ins.Transaction = tx;
                    ins.BindByName = true;
                    ins.CommandText =
                        "INSERT INTO detalle_producto (producto_id, proveedor_cedula) " +
                        "VALUES (:p, :c)";
                    ins.Parameters.Add("p", OracleDbType.Int32).Value = productoId;
                    ins.Parameters.Add("c", OracleDbType.Varchar2).Value = cedulaProveedor;
                    ins.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        catch (OracleException ex)
        {
            throw TraductorErroresOracle.Traducir(ex);
        }
    }
}
