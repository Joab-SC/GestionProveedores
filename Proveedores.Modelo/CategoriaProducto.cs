namespace Proveedores.Modelo
{
    public class CategoriaProducto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        // Para que la categoría se muestre con su nombre en ComboBox y listas
        public override string ToString()
        {
            return Nombre;
        }
    }
}