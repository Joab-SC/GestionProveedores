# CLAUDE.md

Lee primero [README.md](README.md): explica qué hace la aplicación, cómo ponerla en marcha, la arquitectura, el modelo de datos y los casos de uso (CU-01 a CU-12). Este archivo agrega las reglas técnicas y convenciones que hay que respetar al modificar el código.

## Comandos

```bash
dotnet build GestionProveedores.slnx            # compilar toda la solución
dotnet run --project GestionProveedores          # ejecutar la app (WinForms, solo Windows)
docker compose -f Database/compose.yaml up -d    # levantar Oracle (lee Database/.env)
```

Recrear el esquema (borra todos los datos):

```bash
sqlplus <APP_USER>/<APP_USER_PASSWORD>@localhost:1521/FREEPDB1 @Database/01_schema.sql
```

No hay proyecto de pruebas automáticas. Para comprobar un cambio, compila y prueba en la app con los usuarios de prueba del README.

## Stack

- .NET 10. `GestionProveedores` usa `net10.0-windows`; las demás capas, `net10.0`.
- `Nullable=enable` e `ImplicitUsings=enable` en todos los proyectos.
- Solución en formato `.slnx`.
- Paquetes: `Oracle.ManagedDataAccess.Core` y `DotNetEnv` solo en **Datos**; `PDFsharp-MigraDoc` solo en **Presentación**.

## Referencias entre proyectos

```
GestionProveedores → Proveedores.Negocio → Proveedores.Datos → Proveedores.Modelo
GestionProveedores → Proveedores.Modelo
```

- Presentación **nunca** referencia a Datos.
- Negocio **nunca** usa `Oracle.*` (`OracleConnection`, `OracleException`, `OracleTransaction`) ni SQL.
- Modelo no depende de nada.

## Reglas por capa

### Presentación (`GestionProveedores/`)

- Solo formularios y controles. Sin SQL ni reglas de negocio; solo llama a servicios.
- Es la única capa que usa `MessageBox`.
- Atrapa `ReglaNegocioException` (y sus derivadas) y muestra `ex.Message`.
- Visibilidad y habilitación de controles con `Sesion.TienePermiso(Permisos.X)`. Nunca preguntar por el tipo de usuario.
- Los formularios secundarios se abren con `ShowDialog()` desde `FrmMenuPrincipal`.
- `Program.cs`: `FrmLogin.ShowDialog()`; si no devuelve `OK`, la app termina; si sí, `Application.Run(new FrmMenuPrincipal())`.
- `DataGridView` de listas: `ReadOnly`, `FullRowSelect`, `MultiSelect = false`, `AutoSizeColumnsMode = Fill`.
- En los CRUD, el id seleccionado se guarda en un campo privado (`0` = registro nuevo). `Guardar` decide entre `Crear` y `Editar` según ese valor.

### Negocio (`Proveedores.Negocio/`)

- **Todo método público de un servicio empieza con `Sesion.Exigir(Permisos.X)`**, incluidas las lecturas (`Permisos.Leer`). Excepción: `AutenticacionServicio` (login y logout).
- Valida los datos (recorta espacios, longitudes, obligatorios, fechas) antes de llamar al DAO.
- Solo lanza `ReglaNegocioException` o sus derivadas. Suele atrapar `DatoDuplicadoException` o `RegistroEnUsoException` del DAO y relanzarlas con un mensaje específico de la entidad.
- `HashContrasena` es `internal`: SHA-256 en hexadecimal minúscula (64 caracteres). La contraseña nunca se guarda en `Sesion` ni se escribe en la bitácora.

### Datos (`Proveedores.Datos/`)

- Todo el SQL vive aquí, escrito a mano.
- Parámetros con nombre (`:cedula`) y `BindByName = true`. **Nunca** concatenar texto del usuario en el SQL.
- Columnas listadas explícitamente; nunca `SELECT *`.
- `using` en conexión, comando y lector. Cada método abre su propia conexión con `Conexion.Crear()` (el driver hace pooling).
- Cada método captura `OracleException` y hace `throw TraductorErroresOracle.Traducir(ex)`:
  - `ORA-00001` → `DatoDuplicadoException`
  - `ORA-02292` → `RegistroEnUsoException`
  - `ORA-02291` → `ReglaNegocioException("La referencia no existe")`
  - otro código → se relanza el error original
- Transacciones (`OracleTransaction`) solo en Datos. Ejemplo: `DetalleProductoDAO.ReemplazarProductosDeProveedor` hace DELETE + INSERTs y `Commit`, o `Rollback` si falla.
- Filtros opcionales en SQL: `(:param IS NULL OR columna = :param)`.

### Modelo (`Proveedores.Modelo/`)

- Entidades (`Ciudad`, `Producto`, `Proveedor`, `Usuario`…), DTOs de vista (`*VistaDto`, `ConsultaDtos.cs`), constantes y excepciones.
- Las entidades que se muestran en un `ComboBox` sobrescriben `ToString()` para devolver el nombre.
- `TiposUsuario`: `Administrador = 1`, `Tradicional = 2`, `Esporadico = 3`.
- `Permisos`: `Crear`, `Leer`, `Actualizar`, `Eliminar`, `GestionarUsuarios`, `VerBitacora` (valores `"CREAR"`, `"LEER"`, etc., iguales a la tabla `permiso`).
- Excepciones: `ReglaNegocioException` (base), `DatoDuplicadoException`, `RegistroEnUsoException`, `PermisoDenegadoException`.

### Tipos que viajan entre capas

Se usan entidades, DTOs y `List<T>`. `DataTable` solo se usa en reportes (`ReporteDAO` / `ReporteServicio`), porque `GeneradorPdf` recibe una tabla genérica. Las consultas (CU-09) devuelven listas de DTOs.

## Convenciones de nombres

| Elemento | Convención | Ejemplo |
|---|---|---|
| DAO | `<Entidad>DAO`: `Listar`, `ObtenerPorId`/`ObtenerPorCedula`, `Insertar`, `Actualizar`, `Eliminar` | `CiudadDAO.Insertar(Ciudad)` |
| Servicio | `<Entidad>Servicio`: `Listar`, `Crear`, `Editar`, `Eliminar` | `CiudadServicio.Crear(string nombre)` |
| Listas con JOIN | `ListarVista()` devuelve `List<<Entidad>VistaDto>` | `ProductoDAO.ListarVista()` |
| Formulario | `Frm<Nombre>` | `FrmCiudades` |
| Controles | prefijo por tipo: `txt`, `cmb`, `dgv`, `btn`, `lbl`, `dtp`, `lst`, `chk`, `mnu` | `btnGuardar` |

Algunos **nombres de archivo** no coinciden con su clase. No los renombres sin que te lo pidan:

| Archivo | Clase |
|---|---|
| `Proveedores.Datos/ProovedorDAO.cs` | `ProveedorDAO` |
| `Proveedores.Negocio/ProovedorServicio.cs` | `ProveedorServicio` |
| `Proveedores.Modelo/ProovedorVistaDto.cs` | `ProveedorVistaDto` |
| `Proveedores.Datos/DetalleProductoDao.cs` | `DetalleProductoDAO` |
| `Proveedores.Datos/TipoUsuarioDao.cs` | `TipoUsuarioDAO` |

## Mapa de servicios

| Servicio | Métodos públicos | Permisos |
|---|---|---|
| `AutenticacionServicio` | `Ingresar(cedula, contrasena)`, `Salir()` | ninguno |
| `CiudadServicio` | `Listar`, `Crear(nombre)`, `Editar(id, nombre)`, `Eliminar(id)` | LEER / CREAR / ACTUALIZAR / ELIMINAR |
| `CategoriaServicio` | `Listar`, `Crear(nombre, descripcion)`, `Editar(id, …)`, `Eliminar(id)` | ídem |
| `ProductoServicio` | `ListarVista`, `Listar`, `Crear(nombre, descripcion, categoriaId)`, `Editar(id, …)`, `Eliminar(id)` | ídem |
| `ProveedorServicio` | `ListarVista`, `Listar`, `ObtenerPorCedula`, `Crear(Proveedor)`, `Editar(Proveedor)`, `Eliminar(cedula)` | ídem |
| `AsignacionServicio` | `ListarProductoIdsDeProveedor(cedula)` (LEER), `Guardar(cedula, List<int>)` (CREAR + ACTUALIZAR + ELIMINAR) | ver columna anterior |
| `ConsultaServicio` | un método `Obtener…()` por consulta, devuelve `List<Dto>` | LEER |
| `ReporteServicio` | un método por reporte con sus filtros, devuelve `DataTable` | LEER |
| `UsuarioServicio` | `ListarVista`, `ObtenerPorCedula`, `Crear(Usuario, contrasena)`, `Editar(Usuario, string? contrasena)`, `Desactivar(cedula)` | GESTIONAR_USUARIOS |
| `TipoUsuarioServicio` | `Listar` | GESTIONAR_USUARIOS |
| `BitacoraServicio` | `ListarTodos(desde, hasta)`, `ListarPorUsuario(cedula, desde, hasta)` | VER_BITACORA |

`Sesion` (estática): `UsuarioActual`, `BitacoraId`, `Iniciar(Usuario, IEnumerable<string>, int)`, `Cerrar()`, `TienePermiso(string)`, `Exigir(string)`.

## Configuración y conexión

- `Proveedores.Datos/Conexion.cs` carga la configuración una sola vez (`Lazy<T>`). Busca `.env` o `Database/.env` subiendo desde `AppContext.BaseDirectory`.
- `Env.Load(ruta, new LoadOptions(setEnvVars: true, clobberExistingVars: false, onlyExactPath: true))`: las variables de entorno reales tienen prioridad sobre el archivo.
- Variables obligatorias para la app: `ORACLE_HOST`, `ORACLE_PORT`, `ORACLE_SERVICE`, `APP_USER`, `APP_USER_PASSWORD`. `ORACLE_PASSWORD` solo la usa Docker.
- `GestionProveedores.csproj` copia `Database/.env` a la salida con `PreserveNewest`. Después de editarlo, hay que recompilar.
- Si la primera carga falla, `Lazy` guarda la excepción: hay que reiniciar la app.
- **Nunca** pongas credenciales en el código ni hagas commit de `Database/.env`. Solo `Database/.env.example` se sube, con valores de ejemplo.

## Base de datos

- Todo el esquema está en `Database/01_schema.sql`. Es idempotente: borra las tablas existentes y las vuelve a crear. Se ejecuta como `APP_USER`, no como SYS.
- Si cambias una tabla, actualiza el script y también los DAOs afectados.
- `detalle_producto` tiene `ON DELETE CASCADE` hacia `producto` y `proveedor`.
- `uq_un_solo_admin`: índice único que permite un solo usuario con `tipo_usuario_id = 1`. Negocio también lo valida antes con `UsuarioDAO.ExisteAdministrador()`.
- Los usuarios no se borran (tienen filas en `bitacora`); se desactivan con `activo = 0`.
- Contraseñas de los datos de prueba: están comentadas en el script, junto a los `INSERT INTO usuario`.

## Reglas de negocio que no se deben romper

- Mensaje de login siempre igual (`AutenticacionServicio.Mensaje`), sin revelar qué dato falló. Comparación de hash en tiempo fijo (`CryptographicOperations.FixedTimeEquals`).
- `Salir()` se puede llamar varias veces sin error; `FrmMenuPrincipal` lo llama desde el menú y desde `FormClosing`.
- La bitácora es solo lectura en la app. `fecha_salida = NULL` se muestra como "sesión sin cierre".
- La asignación de productos (CU-08) se guarda en una sola transacción.
- Ningún texto `ORA-xxxxx` debe llegar a la pantalla.

## Git

- Mensajes de commit en español con prefijo convencional: `feat:`, `refactor:`, `fix:`. Ejemplo: `feat: agregar servicio para ver y filtrar bitacoras`.
- Commits pequeños, uno por cambio lógico.
