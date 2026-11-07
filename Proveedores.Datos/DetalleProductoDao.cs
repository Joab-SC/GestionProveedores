using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos;

public class DetalleProductoDAO
{
    public List<DetalleProducto> ListarDeProveedor(string cedula)
    {
        var lista = new List<DetalleProducto>();
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.BindByName = true;
            cmd.CommandText =
                "SELECT id, producto_id, proveedor_cedula, precio " +
                "FROM detalle_producto WHERE proveedor_cedula = :cedula";
            cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new DetalleProducto
                {
                    Id = rd.GetInt32(0),
                    ProductoId = rd.GetInt32(1),
                    ProveedorCedula = rd.GetString(2),
                    Precio = rd.GetDecimal(3)
                });
            }
        }
        catch (OracleException ex)
        {
            throw TraductorErroresOracle.Traducir(ex);
        }
        return lista;
    }

    public void ReemplazarProductosDeProveedor(string cedulaProveedor, List<DetalleProducto> asignaciones)
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

                // 2) Insertar las nuevas, una por una, con su precio
                foreach (var asignacion in asignaciones)
                {
                    using var ins = con.CreateCommand();
                    ins.Transaction = tx;
                    ins.BindByName = true;
                    ins.CommandText =
                        "INSERT INTO detalle_producto (producto_id, proveedor_cedula, precio) " +
                        "VALUES (:p, :c, :precio)";
                    ins.Parameters.Add("p", OracleDbType.Int32).Value = asignacion.ProductoId;
                    ins.Parameters.Add("c", OracleDbType.Varchar2).Value = cedulaProveedor;
                    ins.Parameters.Add("precio", OracleDbType.Decimal).Value = asignacion.Precio;
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