namespace Proveedores.Modelo
{
    public class TipoUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Para que el tipo se muestre con su nombre en ComboBox y listas
        public override string ToString()
        {
            return Nombre;
        }
    }
}
