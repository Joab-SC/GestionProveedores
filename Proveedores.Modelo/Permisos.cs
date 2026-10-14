namespace Proveedores.Modelo
{
    // Valores fijos de permiso.nombre (semilla de Database/01_schema.sql).
    // Los formularios y servicios siempre preguntan por permisos,
    // nunca por "tipo de usuario".
    public static class Permisos
    {
        public const string Crear = "CREAR";
        public const string Leer = "LEER";
        public const string Actualizar = "ACTUALIZAR";
        public const string Eliminar = "ELIMINAR";
        public const string GestionarUsuarios = "GESTIONAR_USUARIOS";
        public const string VerBitacora = "VER_BITACORA";
    }
}
