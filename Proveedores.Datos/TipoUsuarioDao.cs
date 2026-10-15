using System;
using System.Collections.Generic;
using System.Text;

using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos;

public class TipoUsuarioDAO
{
    public List<TipoUsuario> Listar()
    {
        const string sql = "SELECT id, nombre FROM tipo_usuario ORDER BY id";
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            using var rd = cmd.ExecuteReader();

            var lista = new List<TipoUsuario>();
            while (rd.Read())
                lista.Add(new TipoUsuario { Id = Convert.ToInt32(rd.GetValue(0)), Nombre = rd.GetString(1) });
            return lista;
        }
        catch (OracleException ex) { throw TraductorErroresOracle.Traducir(ex); }
    }
}