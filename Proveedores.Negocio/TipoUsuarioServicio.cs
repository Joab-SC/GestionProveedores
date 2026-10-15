using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio;

public class TipoUsuarioServicio
{
    private readonly TipoUsuarioDAO dao = new();

    public List<TipoUsuario> Listar()
    {
        Sesion.Exigir(Permisos.GestionarUsuarios);
        return dao.Listar();
    }
}