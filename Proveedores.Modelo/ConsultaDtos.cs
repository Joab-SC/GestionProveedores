using System.ComponentModel;

namespace Proveedores.Modelo;

public record ProveedorProductoDto(
    [property: DisplayName("Cédula")] string Cedula,
    [property: DisplayName("Proveedor")] string Proveedor,
    [property: DisplayName("Ciudad")] string Ciudad,
    [property: DisplayName("Producto")] string Producto);

public record ProductoPorCategoriaDto(
    [property: DisplayName("Categoría")] string Categoria,
    [property: DisplayName("Producto")] string Producto,
    [property: DisplayName("Proveedores")] int Proveedores);

public record UsuarioPermisoDto(
    [property: DisplayName("Cédula")] string Cedula,
    [property: DisplayName("Usuario")] string Usuario,
    [property: DisplayName("Ciudad")] string Ciudad,
    [property: DisplayName("Tipo")] string Tipo,
    [property: DisplayName("Permiso")] string Permiso);

public record ProveedorResumenDto(
    [property: DisplayName("Cédula")] string Cedula,
    [property: DisplayName("Nombre")] string Nombre,
    [property: DisplayName("Apellido")] string Apellido);

public record ProductoSinProveedorDto(
    [property: DisplayName("Id")] int Id,
    [property: DisplayName("Nombre")] string Nombre,
    [property: DisplayName("Descripción")] string? Descripcion);