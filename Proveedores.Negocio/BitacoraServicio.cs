using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio;

public class BitacoraServicio
{
    private readonly BitacoraDAO dao = new();

    public List<BitacoraVistaDto> ListarTodos(DateTime desde, DateTime hasta)
    {
        Sesion.Exigir(Permisos.VerBitacora);
        ValidarFechas(desde, hasta);
        return dao.ListarTodos(desde, hasta);
    }

    public List<BitacoraVistaDto> ListarPorUsuario(string cedulaUsuario, DateTime desde, DateTime hasta)
    {
        Sesion.Exigir(Permisos.VerBitacora);
        ValidarFechas(desde, hasta);

        if (string.IsNullOrWhiteSpace(cedulaUsuario))
            throw new ReglaNegocioException("Debes seleccionar un usuario.");

        return dao.ListarPorUsuario(cedulaUsuario, desde, hasta);
    }

    private static void ValidarFechas(DateTime desde, DateTime hasta)
    {
        if (desde.Date > hasta.Date)
            throw new ReglaNegocioException("La fecha 'desde' no puede ser posterior a 'hasta'.");
    }
}
