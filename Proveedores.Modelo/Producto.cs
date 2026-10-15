namespace Proveedores.Modelo;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
}