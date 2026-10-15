using System;
using System.Collections.Generic;
using System.Text;

namespace Proveedores.Modelo;
public class DetalleProducto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string ProveedorCedula { get; set; } = string.Empty;
}