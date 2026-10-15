

using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio;

public class UsuarioServicio
{
    private readonly UsuarioDAO dao = new();

    public List<UsuarioVistaDto> ListarVista()
    {
        Sesion.Exigir(Permisos.GestionarUsuarios);
        return dao.ListarVista();
    }

    // El formulario lo usa al seleccionar una fila (el DTO no trae ids ni contraseña)
    public Usuario? ObtenerPorCedula(string cedula)
    {
        Sesion.Exigir(Permisos.GestionarUsuarios);
        return dao.ObtenerPorCedula(cedula);
    }

    public void Crear(Usuario u, string contrasena)
    {
        Sesion.Exigir(Permisos.GestionarUsuarios);
        ValidarCampos(u);

        if (string.IsNullOrWhiteSpace(contrasena))
            throw new ReglaNegocioException("La contraseña es obligatoria.");

        if (dao.ObtenerPorCedula(u.Cedula) != null)
            throw new DatoDuplicadoException("Ya existe un usuario con esa cédula.");

        if (u.TipoUsuarioId == TiposUsuario.Administrador && dao.ExisteAdministrador())
            throw new ReglaNegocioException("Ya existe un administrador. Solo puede haber uno.");

        u.Contrasena = HashContrasena.Calcular(contrasena);
        u.Activo = true;
        dao.Insertar(u);
    }

    public void Editar(Usuario u, string? contrasena)
    {
        Sesion.Exigir(Permisos.GestionarUsuarios);
        ValidarCampos(u);

        var actual = dao.ObtenerPorCedula(u.Cedula)
            ?? throw new ReglaNegocioException("El usuario no existe.");

        bool eraAdmin = actual.TipoUsuarioId == TiposUsuario.Administrador;
        bool seraAdmin = u.TipoUsuarioId == TiposUsuario.Administrador;

        if (eraAdmin && !seraAdmin)
            throw new ReglaNegocioException("No se puede cambiar el tipo del administrador.");

        if (!eraAdmin && seraAdmin && dao.ExisteAdministrador())
            throw new ReglaNegocioException("Ya existe un administrador. Solo puede haber uno.");

        if (!u.Activo)
            ValidarPuedeDesactivar(actual);

        u.Contrasena = string.IsNullOrWhiteSpace(contrasena)
            ? actual.Contrasena                       // vacía = no cambiarla
            : HashContrasena.Calcular(contrasena);

        dao.Actualizar(u);
    }

    public void Desactivar(string cedula)
    {
        Sesion.Exigir(Permisos.GestionarUsuarios);

        var actual = dao.ObtenerPorCedula(cedula)
            ?? throw new ReglaNegocioException("El usuario no existe.");

        ValidarPuedeDesactivar(actual);
        dao.CambiarEstado(cedula, false);
    }

    // ---------- reglas privadas ----------

    private static void ValidarPuedeDesactivar(Usuario objetivo)
    {
        if (objetivo.TipoUsuarioId == TiposUsuario.Administrador)
            throw new ReglaNegocioException("No se puede desactivar al administrador.");

        if (Sesion.UsuarioActual?.Cedula == objetivo.Cedula)
            throw new ReglaNegocioException("No puedes desactivarte a ti mismo.");
    }

    private static void ValidarCampos(Usuario u)
    {
        u.Cedula = (u.Cedula ?? "").Trim();
        u.Nombre = (u.Nombre ?? "").Trim();
        u.Apellido = (u.Apellido ?? "").Trim();

        if (!u.Cedula.All(char.IsAsciiDigit))
            throw new ReglaNegocioException("La cédula debe tener solo dígitos");

        if (u.Nombre.Length == 0 || u.Apellido.Length == 0)
            throw new ReglaNegocioException("Nombre y apellido son obligatorios.");

        if (u.Nombre.Length > 60 || u.Apellido.Length > 60)
            throw new ReglaNegocioException("Nombre y apellido admiten máximo 60 caracteres.");

        var hoy = DateTime.Today;
        if (u.FechaNacimiento.Date > hoy)
            throw new ReglaNegocioException("La fecha de nacimiento no puede ser futura.");

        int edad = hoy.Year - u.FechaNacimiento.Year;
        if (u.FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
        if (edad < 18)
            throw new ReglaNegocioException("El usuario debe tener al menos 18 años.");

        if (u.CiudadId <= 0)
            throw new ReglaNegocioException("Debes elegir una ciudad.");

        if (u.TipoUsuarioId <= 0)
            throw new ReglaNegocioException("Debes elegir un tipo de usuario.");
    }
}