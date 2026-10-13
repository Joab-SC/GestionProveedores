namespace Proveedores.Modelo
{
    public class Ciudad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Para que la ciudad se muestre con su nombre en ComboBox y listas
        public override string ToString()
        {
            return Nombre;
        }
    }
}