# Prompt: GestionProveedores — Casos de uso a implementar
 
Eres un desarrollador C# senior. Implementa los casos de uso de este documento respetando el contexto, la arquitectura y las convenciones que se describen primero. Trabaja un caso de uso a la vez, en el orden de la sección "Orden de implementación", y no avances al siguiente hasta que se cumpla su criterio "Listo cuando…".
 
---
 
## 1. Contexto del proyecto
 
Aplicación de escritorio para gestionar **proveedores, productos y categorías**, con control de acceso por tipo de usuario, bitácora de sesiones, consultas y reportes en PDF.
 
| Componente | Tecnología |
|---|---|
| Framework | .NET 10 (`net10.0`; la app WinForms usa `net10.0-windows`) |
| Interfaz | Windows Forms |
| Base de datos | Oracle Free (`gvenzl/oracle-free`) en Docker Compose, servicio `FREEPDB1`, puerto 1521 |
| Driver | `Oracle.ManagedDataAccess.Core` 23.x (solo en Datos) |
| PDF | `PDFsharp-MigraDoc` (solo en Presentación) |
| Lenguaje | C#, `Nullable=enable`, `ImplicitUsings=enable` |
| Solución | `GestionProveedores.slnx` |
 
### Estructura de la solución
 
```
GestionProveedores.slnx
compose.yaml
├── GestionProveedores/        ← Presentación (WinForms)
├── Proveedores.Negocio/       ← Reglas de negocio y servicios
├── Proveedores.Datos/         ← DAO y acceso a Oracle
└── Proveedores.Modelo/        ← Entidades, DTO, constantes y excepciones
```
 
Referencias permitidas (un solo sentido):
 
```
GestionProveedores → Proveedores.Negocio → Proveedores.Datos → Proveedores.Modelo
GestionProveedores → Proveedores.Modelo
```
 
Presentación **nunca** referencia a Datos. Negocio **nunca** referencia a Oracle.
 
---
 
## 2. Reglas de arquitectura (valen para todos los casos de uso)
 
1. **Presentación:** formularios y controles. No contiene SQL ni reglas de negocio. Solo habla con servicios de Negocio. Es la única capa que usa `MessageBox`.
2. **Negocio:** servicios con validaciones y reglas. Cada método que escribe empieza con `Sesion.Exigir(...)`. No usa `OracleConnection`, `OracleException` ni SQL.
3. **Datos:** clases DAO con todo el SQL. Usa parámetros con nombre (`:nombre`, `BindByName = true`), columnas listadas (nunca `SELECT *`) y `using` en conexión, comando y lector. Captura `OracleException` y la traduce con `TraductorErroresOracle`.
4. **Modelo:** entidades, DTO de vista, constantes y excepciones. No depende de ninguna otra capa.
5. **Entidades y DTO** viajan entre capas. `DataTable` solo se acepta en consultas (CU-09) y reportes (CU-10).
6. La cadena de conexión no lleva credenciales en el código: `Conexion.Crear()` carga las variables de entorno requeridas desde `.env` sin sobrescribir valores externos.
7. La contraseña de los usuarios se guarda únicamente como hash SHA-256 (hexadecimal en minúscula, 64 caracteres).
 
### Convenciones de nombres
 
| Elemento | Convención | Ejemplo |
|---|---|---|
| DAO | `<Entidad>DAO` con `Listar`, `ObtenerPorId`, `Insertar`, `Actualizar`, `Eliminar` | `CiudadDAO.Insertar(Ciudad)` |
| Servicio | `<Entidad>Servicio` con `Listar`, `Crear`, `Editar`, `Eliminar` | `CiudadServicio.Crear(string nombre)` |
| Formulario | `Frm<Nombre>` | `FrmCiudades` |
| Controles | prefijo por tipo: `txt`, `cmb`, `dgv`, `btn`, `lbl`, `dtp`, `lst`, `chk`, `mnu` | `btnGuardar`, `dgvCiudades` |
| Entidad con ComboBox | `ToString()` devuelve el nombre | `Ciudad.ToString()` |
 
---
 
## 3. Modelo de datos
 
Todas las tablas viven en un único script `Database/01_schema.sql` que se ejecuta con el usuario de la aplicación. Se implementa en CU-00.
 
| Tabla | Columnas | Restricciones |
|---|---|---|
| `ciudad` | `id` (identidad), `nombre` | PK `id`; `nombre` único, no nulo, máx. 80 |
| `categoria_producto` | `id`, `nombre`, `descripcion` | PK `id`; `nombre` no nulo; `descripcion` máx. 250 |
| `producto` | `id`, `nombre`, `descripcion`, `categoria_id` | PK `id`; FK a `categoria_producto` |
| `proveedor` | `cedula`, `nombre`, `apellido`, `fecha_nacimiento`, `ciudad_id` | PK `cedula`; FK a `ciudad` |
| `detalle_producto` | `id`, `producto_id`, `proveedor_cedula` | PK `id`; FK a `producto` y `proveedor` con `ON DELETE CASCADE`; par (`producto_id`, `proveedor_cedula`) único |
| `tipo_usuario` | `id`, `nombre` | PK `id`; datos iniciales: 1 Administrador, 2 Tradicional, 3 Esporádico |
| `permiso` | `id`, `nombre` | PK `id`; datos iniciales: `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR`, `GESTIONAR_USUARIOS`, `VER_BITACORA` |
| `detalle_tipo_usuario` | `tipo_usuario_id`, `permiso_id` | PK compuesta; FK a ambas tablas |
| `usuario` | `cedula`, `nombre`, `apellido`, `fecha_nacimiento`, `ciudad_id`, `tipo_usuario_id`, `contrasena`, `activo` | PK `cedula`; FK a `ciudad` y `tipo_usuario`; `activo` 0/1; índice único que permite **un solo** administrador |
| `bitacora` | `id`, `usuario_cedula`, `fecha_entrada`, `fecha_salida` | PK `id`; FK a `usuario`; `fecha_salida` admite nulo |
 
**Matriz de permisos (datos iniciales de `detalle_tipo_usuario`)**
 
| Permiso | Administrador | Tradicional | Esporádico |
|---|:---:|:---:|:---:|
| `CREAR` | ✅ | ✅ | |
| `LEER` | ✅ | ✅ | ✅ |
| `ACTUALIZAR` | ✅ | ✅ | |
| `ELIMINAR` | ✅ | ✅ | |
| `GESTIONAR_USUARIOS` | ✅ | | |
| `VER_BITACORA` | ✅ | | |
 
Datos de prueba mínimos: varias ciudades, categorías, productos y proveedores con asignaciones; un administrador, un Tradicional y un Esporádico (con hash SHA-256 de su contraseña).
 
---
 
## 4. Resumen de casos de uso
 
| CU | Caso de uso | Administrador | Tradicional | Esporádico | Permiso que exige |
|---|---|:---:|:---:|:---:|---|
| 00 | Base común del proyecto | (desarrollo) | | | ninguno |
| 01 | Iniciar sesión | ✅ | ✅ | ✅ | ninguno |
| 02 | Cerrar sesión | ✅ | ✅ | ✅ | ninguno |
| 03 | Navegar el menú principal | ✅ | ✅ | ✅ | ninguno |
| 04 | CRUD de Ciudades | ✅ | ✅ | | `CREAR`, `ACTUALIZAR`, `ELIMINAR` |
| 05 | CRUD de Categorías | ✅ | ✅ | | `CREAR`, `ACTUALIZAR`, `ELIMINAR` |
| 06 | CRUD de Productos | ✅ | ✅ | | `CREAR`, `ACTUALIZAR`, `ELIMINAR` |
| 07 | CRUD de Proveedores | ✅ | ✅ | | `CREAR`, `ACTUALIZAR`, `ELIMINAR` |
| 08 | Asignar productos a proveedores | ✅ | ✅ | | `CREAR`, `ACTUALIZAR`, `ELIMINAR` |
| 09 | Ejecutar consultas | ✅ | ✅ | ✅ | `LEER` |
| 10 | Generar reportes en PDF | ✅ | ✅ | ✅ | `LEER` |
| 11 | Gestionar usuarios | ✅ | | | `GESTIONAR_USUARIOS` |
| 12 | Consultar bitácora | ✅ | | | `VER_BITACORA` |
 
Cada caso de uso trae: **Actor y permiso**, **Qué hace el usuario**, **Pasos de construcción**, **Reglas y errores** y **Listo cuando…**.
 
---
 
## 5. Contratos entre casos de uso
 
Los casos de uso se apoyan unos en otros. Estas firmas son el acuerdo entre ellos: respétalas exactamente, y si un caso depende de una que aún no existe, créala con ese nombre y firma.
 
**Modelo**
 
| Elemento | Contenido |
|---|---|
| `TiposUsuario` | constantes `Administrador = 1`, `Tradicional = 2`, `Esporadico = 3` |
| `Permisos` | constantes de texto `Crear`, `Leer`, `Actualizar`, `Eliminar`, `GestionarUsuarios`, `VerBitacora`, con los valores `CREAR`, `LEER`, etc. |
| Excepciones | `ReglaNegocioException` (base), `DatoDuplicadoException`, `RegistroEnUsoException` y `PermisoDenegadoException`, todas derivadas de la primera |
 
**Negocio**
 
| Clase | Métodos |
|---|---|
| `Sesion` (estática) | `UsuarioActual`, `BitacoraId`, `Iniciar(Usuario, IEnumerable<string> permisos, int bitacoraId)`, `Cerrar()`, `TienePermiso(string)`, `Exigir(string)` |
| `AutenticacionServicio` | `Ingresar(string cedula, string contrasena)`, `Salir()` |
| `CiudadServicio` | `Listar()`, `Crear(string nombre)`, `Editar(int id, string nombre)`, `Eliminar(int id)` |
| `CategoriaServicio` | `Listar()`, `Crear(string nombre, string descripcion)`, `Editar(int id, string nombre, string descripcion)`, `Eliminar(int id)` |
| `ProductoServicio` | `ListarVista()`, `Crear(string nombre, string descripcion, int categoriaId)`, `Editar(int id, ...)`, `Eliminar(int id)` |
| `ProveedorServicio` | `ListarVista()`, `Listar()`, `Crear(Proveedor)`, `Editar(Proveedor)`, `Eliminar(string cedula)` |
| `AsignacionServicio` | `ListarProductoIdsDeProveedor(string cedula)`, `Guardar(string cedula, List<int> productoIds)` |
| `UsuarioServicio` | `ListarVista()`, `Crear(Usuario, string contrasena)`, `Editar(Usuario, string? contrasena)`, `Desactivar(string cedula)` |
| `BitacoraServicio` | `Listar(string? cedulaUsuario, DateTime desde, DateTime hasta)` |
| `ConsultaServicio` | un método por consulta, devuelve `DataTable` |
| `ReporteServicio` | un método por reporte con sus filtros, devuelve `DataTable` |
 
**Quién usa qué**
 
| Caso de uso | Necesita de otros |
|---|---|
| CU-01 Iniciar sesión | `Sesion`, `Permisos`, excepciones, tablas de usuario, permiso y bitácora |
| CU-03 Menú | `Sesion.TienePermiso`, `Permisos` |
| CU-06 Productos | `CategoriaServicio.Listar()` |
| CU-07 Proveedores | `CiudadServicio.Listar()` |
| CU-08 Asignaciones | `ProveedorServicio.Listar()` y `ProductoServicio.Listar()` |
| CU-11 Usuarios | `CiudadServicio.Listar()`, tipos de usuario, `Sesion.UsuarioActual` |
| CU-12 Bitácora | `UsuarioServicio.ListarVista()` y las filas que generan CU-01 y CU-02 |
 
---
 
## 6. Casos de uso
 
### CU-00 Base común del proyecto
 
**Actor:** quien desarrolla. Todos los demás casos de uso dependen de este.
 
**Pasos de construcción**
 
1. Crea `Database/01_schema.sql` con las tablas, restricciones, secuencias o identidades, datos iniciales (tipos de usuario, permisos y matriz) y datos de prueba descritos en la sección 3. Debe poder ejecutarse completo sobre una base vacía.
2. En **Modelo** crea las constantes `TiposUsuario` y `Permisos`.
3. En **Modelo** crea las excepciones: `ReglaNegocioException`, `DatoDuplicadoException`, `RegistroEnUsoException` y `PermisoDenegadoException`.
4. En **Datos** crea `TraductorErroresOracle`:
   - `ORA-00001` → `DatoDuplicadoException`
   - `ORA-02292` → `RegistroEnUsoException`
   - `ORA-02291` → `ReglaNegocioException("La referencia no existe")`
   - cualquier otro error se relanza.
5. En **Datos**, `Conexion.Crear()` obtiene sus parámetros de variables de entorno (ver regla 6 de la sección 2).
6. En **Negocio** crea la clase estática `Sesion` con la firma del contrato. `Exigir` lanza `PermisoDenegadoException` si el usuario no tiene el permiso.
7. Cada DAO captura `OracleException` y lanza `TraductorErroresOracle.Traducir(ex)`. Los servicios solo lanzan `ReglaNegocioException` o sus derivadas.
8. Crea el `.gitignore` (`bin/`, `obj/`, `.vs/`, `.freebuff/`).
 
**Listo cuando…** el script crea toda la base desde cero, el proyecto compila y `Proveedores.Negocio` no tiene ningún `using Oracle...`.
 
---
 
### CU-01 Iniciar sesión
 
**Actor:** cualquier usuario. **Permiso:** ninguno.
 
**Qué hace el usuario**
 
1. Abre la aplicación y aparece `FrmLogin`.
2. Escribe su cédula y su contraseña, y pulsa **Ingresar** (o Enter).
3. Si los datos son correctos, se abre el menú principal. Si no, ve el mensaje *"Cédula o contraseña incorrectos."*
 
**Pasos de construcción**
 
1. **Modelo:** entidades `Usuario`, `TipoUsuario`, `Permiso` y `Bitacora`.
2. **Datos:**
   - `UsuarioDAO.ObtenerPorCedula`.
   - `PermisoDAO.ListarPorTipoUsuario`.
   - `BitacoraDAO.RegistrarEntrada`, que inserta la fila y devuelve el id.
3. **Negocio:** `AutenticacionServicio.Ingresar(cedula, contrasena)`:
   - Busca el usuario por cédula.
   - Rechaza si no existe, si está inactivo o si la contraseña no coincide. El mensaje es siempre el mismo y no revela cuál dato falló.
   - Compara el hash SHA-256 con `usuario.contrasena`.
   - Carga los permisos del tipo de usuario, registra la entrada en la bitácora y llama `Sesion.Iniciar(...)`.
4. **Presentación:** `FrmLogin` con `txtCedula`, `txtContrasena` (`UseSystemPasswordChar = true`), `lblMensaje` (rojo), `btnIngresar` (`AcceptButton`) y `btnSalir` (`CancelButton`).
5. **Presentación:** `btnIngresar_Click` valida campos vacíos, llama al servicio y responde `DialogResult.OK`. Atrapa `ReglaNegocioException` y muestra `ex.Message` en `lblMensaje`.
6. **Presentación:** `Program.cs` muestra `FrmLogin` con `ShowDialog()`. Si el resultado no es `OK`, la aplicación termina; si lo es, ejecuta `Application.Run(new FrmMenuPrincipal())`.
7. Opcional: limita a 3 intentos antes de cerrar la aplicación.
 
**Reglas y errores**
 
- La contraseña nunca aparece en mensajes ni en la bitácora.
- Un usuario inactivo (`activo = 0`) no puede ingresar.
 
**Listo cuando…** entra cada uno de los 3 tipos de usuario, un usuario inactivo es rechazado y la bitácora registra una fila nueva en cada ingreso.
 
---
 
### CU-02 Cerrar sesión
 
**Actor:** cualquier usuario con sesión iniciada. **Permiso:** ninguno.
 
**Qué hace el usuario**
 
1. Elige **Sesión → Cerrar sesión** en el menú, o cierra la ventana con la X.
2. La aplicación registra la hora de salida y termina.
 
**Pasos de construcción**
 
1. **Datos:** `BitacoraDAO.RegistrarSalida(id)` con `UPDATE ... SET fecha_salida = SYSTIMESTAMP WHERE id = :id AND fecha_salida IS NULL`.
2. **Negocio:** `AutenticacionServicio.Salir()` llama `BitacoraDAO.RegistrarSalida(Sesion.BitacoraId)` y después `Sesion.Cerrar()`.
3. **Presentación:** la opción `mnuCerrarSesion` de `FrmMenuPrincipal` llama `Salir()` y cierra el formulario.
4. **Presentación:** el evento `FormClosing` de `FrmMenuPrincipal` también llama `Salir()`, para que la X registre la salida.
5. `Salir()` no debe fallar si ya se llamó antes.
 
**Reglas y errores**
 
- Si el programa se cae, la fila queda con `fecha_salida` en `NULL`. Es válido y la bitácora lo muestra como *"sesión sin cierre"*.
- La bitácora no se edita ni se borra desde la aplicación.
 
**Listo cuando…** cada ingreso con salida normal deja una fila con entrada y salida.
 
---
 
### CU-03 Navegar el menú principal según permisos
 
**Actor:** cualquier usuario con sesión iniciada. **Permiso:** depende de cada opción.
 
**Qué hace el usuario**
 
1. Tras el login ve el menú con solo las opciones que su tipo de usuario puede usar.
2. Elige una opción y se abre su formulario con `ShowDialog()`.
3. Abajo ve su nombre y su tipo de usuario.
 
**Visibilidad de las opciones**
 
| Opción del menú | Se muestra si el usuario tiene… |
|---|---|
| Entidades y Transacciones | `CREAR`, `ACTUALIZAR` o `ELIMINAR` |
| Consultas y Reportes | `LEER` |
| Auditoría → Usuarios | `GESTIONAR_USUARIOS` |
| Auditoría → Bitácora | `VER_BITACORA` |
| Sesión → Cerrar sesión | siempre |
 
**Pasos de construcción**
 
1. **Presentación:** `FrmMenuPrincipal` con un `MenuStrip`: **Entidades** (Ciudades, Categorías, Productos, Proveedores), **Transacciones** (Asignar productos a proveedores), **Consultas**, **Reportes**, **Auditoría** (Usuarios, Bitácora) y **Sesión** (Cerrar sesión).
2. **Presentación:** un `StatusStrip` con `lblUsuario` que muestre el nombre y el tipo de `Sesion.UsuarioActual`.
3. **Presentación:** en el evento `Load`, asigna `Visible` a cada opción con `Sesion.TienePermiso(...)` según la tabla anterior.
4. **Presentación:** cada opción abre su formulario con `ShowDialog()`.
5. Los formularios nunca preguntan *"¿es administrador?"*; siempre preguntan por un permiso.
 
**Reglas y errores**
 
- Ocultar opciones es solo comodidad. La seguridad real está en Negocio: cada método que escribe llama `Sesion.Exigir(...)`.
 
**Listo cuando…** cada tipo de usuario ve exactamente las opciones de la tabla y ninguna más.
 
---
 
### CU-04 Gestionar ciudades (CRUD)
 
**Actor:** Administrador y Tradicional. **Permisos:** `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR`.
 
Este es el **patrón base** de todos los CRUD. Los CU-05, CU-06 y CU-07 lo repiten con sus propios campos.
 
**Qué hace el usuario**
 
1. Abre **Entidades → Ciudades** y ve la lista de ciudades.
2. Para crear: escribe el nombre y pulsa **Guardar**.
3. Para editar: selecciona una fila, cambia el nombre y pulsa **Guardar**.
4. Para eliminar: selecciona una fila, pulsa **Eliminar** y confirma con Sí.
5. **Nuevo** limpia los campos para empezar otro registro.
 
**Pasos de construcción**
 
1. **Modelo:** `Ciudad` (`Id`, `Nombre`) con `ToString()` que devuelva el nombre.
2. **Datos:** `CiudadDAO` con `Listar`, `ObtenerPorId`, `Insertar`, `Actualizar` y `Eliminar`, con SQL parametrizado.
3. **Negocio:** `CiudadServicio.Crear` y `Editar` llaman `Sesion.Exigir(Permisos.Crear)` y `Sesion.Exigir(Permisos.Actualizar)` respectivamente, validan y delegan en el DAO. `Eliminar` llama `Sesion.Exigir(Permisos.Eliminar)`. `Listar` exige `Permisos.Leer`.
4. **Presentación:** `FrmCiudades` con `dgvCiudades`, `txtNombre`, `btnNuevo`, `btnGuardar` y `btnEliminar`. Tabla en `ReadOnly`, `FullRowSelect`, `MultiSelect = false` y `AutoSizeColumnsMode = Fill`.
5. **Presentación:** al abrir, llena la tabla con `Listar()` y limpia los campos. Al seleccionar una fila, pasa sus datos a los campos y guarda el id en una variable privada (`0` = registro nuevo).
6. **Presentación:** `btnGuardar_Click` llama a `Crear` si el id es `0` o a `Editar` si no, refresca la tabla y limpia. Ante `ReglaNegocioException` muestra `ex.Message` con `MessageBox`.
7. **Presentación:** `btnEliminar_Click` pide confirmación Sí/No antes de llamar al servicio.
8. **Presentación:** `btnNuevo` y `btnGuardar` se habilitan con `CREAR`/`ACTUALIZAR`; `btnEliminar`, con `ELIMINAR`.
 
```csharp
private void btnGuardar_Click(object sender, EventArgs e)
{
    try
    {
        if (idSeleccionado == 0) servicio.Crear(txtNombre.Text);
        else servicio.Editar(idSeleccionado, txtNombre.Text);
 
        Refrescar();
        Limpiar();
    }
    catch (ReglaNegocioException ex)
    {
        MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
```
 
**Reglas y errores**
 
- Nombre obligatorio (no vacío ni solo espacios), máximo 80 caracteres.
- Nombre duplicado: *"Ya existe una ciudad con ese nombre."*
- Ciudad con proveedores o usuarios: *"No se puede eliminar: hay proveedores o usuarios en esta ciudad."*
- Nunca aparece un texto como `ORA-02292` en pantalla.
 
**Listo cuando…** crea, edita y elimina ciudades con mensajes claros.
 
---
 
### CU-05 Gestionar categorías (CRUD)
 
**Actor:** Administrador y Tradicional. **Permisos:** `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR`. Sigue el patrón del CU-04.
 
**Qué hace el usuario**
 
1. Abre **Entidades → Categorías** y ve la lista.
2. Escribe nombre y descripción, y pulsa **Guardar** para crear.
3. Selecciona una fila para editarla y pulsa **Guardar**.
4. Pulsa **Eliminar** y confirma para borrar una categoría.
 
**Pasos de construcción**
 
1. **Modelo:** `CategoriaProducto` (`Id`, `Nombre`, `Descripcion`) con `ToString()` que devuelva el nombre.
2. **Datos:** `CategoriaDAO` con `Listar`, `ObtenerPorId`, `Insertar`, `Actualizar` y `Eliminar`.
3. **Negocio:** `CategoriaServicio` con `Listar`, `Crear`, `Editar` y `Eliminar`, cada uno con su `Sesion.Exigir(...)`.
4. **Negocio:** valida nombre obligatorio (máximo 60) y descripción máximo 250.
5. **Presentación:** `FrmCategorias` con `dgvCategorias`, `txtNombre`, `txtDescripcion`, `btnNuevo`, `btnGuardar` y `btnEliminar`, con el mismo comportamiento del CU-04.
 
**Reglas y errores**
 
- No se puede eliminar una categoría que tiene productos: `RegistroEnUsoException` con un mensaje claro.
 
**Listo cuando…** crea, edita y elimina categorías, y al borrar una con productos aparece un aviso entendible.
 
---
 
### CU-06 Gestionar productos (CRUD)
 
**Actor:** Administrador y Tradicional. **Permisos:** `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR`. Depende de CU-05.
 
**Qué hace el usuario**
 
1. Abre **Entidades → Productos** y ve la lista, con el nombre de la categoría (no su id).
2. Escribe nombre y descripción, elige la categoría en la lista desplegable y pulsa **Guardar**.
3. Selecciona una fila para editar, o pulsa **Eliminar** y confirma. La confirmación avisa que también se borrarán las asignaciones del producto.
 
**Pasos de construcción**
 
1. **Modelo:** `Producto` (`Id`, `Nombre`, `Descripcion`, `CategoriaId`) y el DTO `ProductoVistaDto` (`Id`, `Nombre`, `Descripcion`, `Categoria`).
2. **Datos:** `ProductoDAO` con `ListarVista()` (JOIN con `categoria_producto`), `Listar`, `ObtenerPorId`, `Insertar`, `Actualizar` y `Eliminar`.
3. **Negocio:** `ProductoServicio` con `ListarVista`, `Listar`, `Crear`, `Editar` y `Eliminar`, cada uno con su `Sesion.Exigir(...)`.
4. **Negocio:** valida nombre obligatorio (máximo 60), descripción máximo 250 y **categoría obligatoria**.
5. **Presentación:** `FrmProductos` con `dgvProductos`, `txtNombre`, `txtDescripcion`, `cmbCategoria` (`DropDownStyle = DropDownList`), `btnNuevo`, `btnGuardar` y `btnEliminar`.
6. **Presentación:** llena `cmbCategoria` con `CategoriaServicio.Listar()` y selecciona la correcta al elegir una fila.
7. **Presentación:** la confirmación de eliminar avisa que se borran también las asignaciones a proveedores.
 
**Reglas y errores**
 
- Eliminar un producto borra sus filas en `detalle_producto` (`ON DELETE CASCADE`).
- Una categoría inexistente produce `ReglaNegocioException("La referencia no existe")`.
 
**Listo cuando…** crea, edita y elimina productos, la tabla muestra el nombre de la categoría y no se puede guardar sin categoría.
 
---
 
### CU-07 Gestionar proveedores (CRUD)
 
**Actor:** Administrador y Tradicional. **Permisos:** `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR`. Depende de CU-04.
 
**Qué hace el usuario**
 
1. Abre **Entidades → Proveedores** y ve la lista, con el nombre de la ciudad.
2. Escribe cédula, nombre y apellido, elige la fecha de nacimiento y la ciudad, y pulsa **Guardar**.
3. Al seleccionar una fila, la cédula queda en solo lectura porque es la llave.
4. Pulsa **Eliminar** y confirma. La confirmación avisa que se borrarán también sus asignaciones de productos.
 
**Pasos de construcción**
 
1. **Modelo:** `Proveedor` (`Cedula`, `Nombre`, `Apellido`, `FechaNacimiento`, `CiudadId`) y el DTO `ProveedorVistaDto` (`Cedula`, `NombreCompleto`, `Ciudad`).
2. **Datos:** `ProveedorDAO` con `ListarVista()` (JOIN con `ciudad`), `Listar`, `ObtenerPorCedula`, `Insertar`, `Actualizar` y `Eliminar`.
3. **Negocio:** `ProveedorServicio` con `Crear`, `Editar` y `Eliminar`, cada uno con su `Sesion.Exigir(...)`. Valida:
   - Cédula obligatoria, solo dígitos, entre 6 y 15.
   - Nombre y apellido obligatorios, máximo 60 caracteres.
   - Fecha de nacimiento no futura y edad mínima razonable (por ejemplo, 18 años).
   - Ciudad obligatoria.
4. **Presentación:** `FrmProveedores` con `dgvProveedores`, `txtCedula`, `txtNombre`, `txtApellido`, `dtpFechaNacimiento`, `cmbCiudad`, `btnNuevo`, `btnGuardar` y `btnEliminar`.
5. **Presentación:** `txtCedula.ReadOnly = true` al editar un registro existente y `false` al crear uno nuevo.
6. **Presentación:** llena `cmbCiudad` con `CiudadServicio.Listar()`.
 
**Reglas y errores**
 
- Cédula duplicada: `DatoDuplicadoException` con mensaje claro.
- La cédula es la llave primaria y no se puede cambiar al editar.
- Eliminar un proveedor borra sus asignaciones (`ON DELETE CASCADE`).
 
**Listo cuando…** crea, edita y elimina proveedores, rechaza cédulas repetidas o con letras, y rechaza fechas futuras.
 
---
 
### CU-08 Asignar productos a proveedores (transacción)
 
**Actor:** Administrador y Tradicional. **Permisos:** `CREAR`, `ACTUALIZAR`, `ELIMINAR`. Depende de CU-06 y CU-07. Es la transacción que exige el enunciado.
 
**Qué hace el usuario**
 
1. Abre **Transacciones → Asignar productos a proveedores**.
2. Elige un proveedor en `cmbProveedor`. Las dos listas se llenan solas: a la izquierda los productos que **aún no** ofrece, a la derecha los que ya ofrece.
3. Selecciona un producto y pulsa **Agregar** (pasa a la derecha) o **Quitar** (vuelve a la izquierda).
4. Los cambios solo viven en pantalla hasta pulsar **Guardar cambios**. Entonces se guardan todos juntos o ninguno.
 
**Pasos de construcción**
 
1. **Modelo:** `DetalleProducto` (`Id`, `ProductoId`, `ProveedorCedula`).
2. **Datos:** `DetalleProductoDAO.ListarProductoIdsDeProveedor(cedula)` devuelve los productos ya asignados.
3. **Datos:** `DetalleProductoDAO.ReemplazarProductosDeProveedor(cedula, List<int> productoIds)` con `OracleTransaction`:
   - Abre la conexión y llama `BeginTransaction()`.
   - Ejecuta `DELETE FROM detalle_producto WHERE proveedor_cedula = :cedula`.
   - Por cada id, ejecuta `INSERT INTO detalle_producto (producto_id, proveedor_cedula) VALUES (:p, :c)`.
   - Si todo sale bien, `Commit()`. Si hay `OracleException`, `Rollback()` y lanza `TraductorErroresOracle.Traducir(ex)`.
4. **Negocio:** `AsignacionServicio.Guardar(cedula, productoIds)` llama `Sesion.Exigir(...)`, verifica que el proveedor exista y delega en el DAO. Negocio no conoce `OracleTransaction`.
5. **Presentación:** `FrmAsignaciones` con `cmbProveedor`, `lstDisponibles`, `lstAsignados`, `btnAgregar`, `btnQuitar` y `btnGuardarCambios`.
6. **Presentación:** al cambiar de proveedor, recarga ambas listas (asignados desde el servicio; disponibles = todos los productos menos los asignados).
7. **Presentación:** `btnAgregar` y `btnQuitar` solo mueven elementos entre las listas en memoria.
8. **Presentación:** `btnGuardarCambios_Click` envía la lista de `lstAsignados` al servicio y muestra confirmación o `ex.Message`.
 
```csharp
public void ReemplazarProductosDeProveedor(string cedulaProveedor, List<int> productoIds)
{
    using (var con = Conexion.Crear())
    {
        con.Open();
        using (var tx = con.BeginTransaction())
        {
            try
            {
                // 1) DELETE FROM detalle_producto WHERE proveedor_cedula = :cedula
                // 2) por cada id: INSERT INTO detalle_producto (producto_id, proveedor_cedula) VALUES (:p, :c)
                tx.Commit();
            }
            catch (OracleException ex)
            {
                tx.Rollback();
                throw TraductorErroresOracle.Traducir(ex);
            }
        }
    }
}
```
 
**Reglas y errores**
 
- Todas las escrituras van en **una sola transacción**: o se cumplen todas o no queda ninguna.
- Un error a la mitad no deja al proveedor con datos a medias.
 
**Listo cuando…** se asignan y quitan productos, los cambios se ven al reabrir el formulario, y un error forzado a la mitad deja los datos como estaban.
 
---
 
### CU-09 Ejecutar consultas
 
**Actor:** Administrador, Tradicional y Esporádico. **Permiso:** `LEER`.
 
**Qué hace el usuario**
 
1. Abre **Consultas**.
2. Elige una de las 5 consultas en `cmbConsulta`; `lblDescripcion` explica qué hace.
3. Pulsa **Ejecutar** y ve el resultado en una tabla.
 
**Las 5 consultas**
 
| # | Consulta | Tipo | Tablas |
|---|---|---|---|
| 1 | Proveedores con su ciudad y los productos que ofrecen | JOIN | proveedor, ciudad, detalle_producto, producto (4) |
| 2 | Productos por categoría con su número de proveedores | JOIN | categoria_producto, producto, detalle_producto (3) |
| 3 | Usuarios con su ciudad, tipo y permisos | JOIN | usuario, ciudad, tipo_usuario, detalle_tipo_usuario, permiso (5) |
| 4 | Proveedores que ofrecen más productos que el promedio | Subconsulta | proveedor, detalle_producto |
| 5 | Productos que ningún proveedor ofrece | Subconsulta (`NOT EXISTS`) | producto, detalle_producto |
 
**Pasos de construcción**
 
1. **Datos:** `ConsultaDAO` con un método por consulta. Cada uno devuelve un `DataTable`.
2. **Datos:** escribe el SQL de cada consulta con las columnas listadas.
 
   Consulta 1:
   ```sql
   SELECT pr.cedula, pr.nombre || ' ' || pr.apellido AS proveedor,
          c.nombre AS ciudad, p.nombre AS producto
   FROM proveedor pr
   JOIN ciudad c ON c.id = pr.ciudad_id
   JOIN detalle_producto dp ON dp.proveedor_cedula = pr.cedula
   JOIN producto p ON p.id = dp.producto_id
   ORDER BY proveedor, producto
   ```
 
   Consulta 2:
   ```sql
   SELECT cat.nombre AS categoria, p.nombre AS producto, COUNT(dp.id) AS proveedores
   FROM categoria_producto cat
   JOIN producto p ON p.categoria_id = cat.id
   LEFT JOIN detalle_producto dp ON dp.producto_id = p.id
   GROUP BY cat.nombre, p.nombre
   ORDER BY cat.nombre, p.nombre
   ```
 
   Consulta 3:
   ```sql
   SELECT u.cedula, u.nombre || ' ' || u.apellido AS usuario, c.nombre AS ciudad,
          t.nombre AS tipo, pe.nombre AS permiso
   FROM usuario u
   JOIN ciudad c ON c.id = u.ciudad_id
   JOIN tipo_usuario t ON t.id = u.tipo_usuario_id
   JOIN detalle_tipo_usuario dtu ON dtu.tipo_usuario_id = t.id
   JOIN permiso pe ON pe.id = dtu.permiso_id
   ORDER BY usuario, permiso
   ```
 
   Consulta 4:
   ```sql
   SELECT pr.cedula, pr.nombre, pr.apellido
   FROM proveedor pr
   WHERE (SELECT COUNT(*) FROM detalle_producto dp
          WHERE dp.proveedor_cedula = pr.cedula)
       > (SELECT AVG(n) FROM (SELECT COUNT(*) AS n FROM detalle_producto
                              GROUP BY proveedor_cedula))
   ```
 
   Consulta 5:
   ```sql
   SELECT p.id, p.nombre, p.descripcion
   FROM producto p
   WHERE NOT EXISTS (SELECT 1 FROM detalle_producto dp WHERE dp.producto_id = p.id)
   ```
3. **Negocio:** `ConsultaServicio` con `Sesion.Exigir(Permisos.Leer)` en cada método, que delega en `ConsultaDAO`.
4. **Presentación:** `FrmConsultas` con `cmbConsulta`, `btnEjecutar`, `dgvResultado` y `lblDescripcion`.
5. **Presentación:** al cambiar `cmbConsulta`, actualiza `lblDescripcion`. `btnEjecutar_Click` llama al servicio y asigna el `DataTable` a `dgvResultado`.
 
**Reglas y errores**
 
- La consulta 5 sale vacía si todos los productos tienen proveedor. Para la sustentación, agrega un producto sin proveedor.
 
**Listo cuando…** las 5 consultas muestran resultados y un usuario Esporádico puede ejecutarlas.
 
---
 
### CU-10 Generar reportes en PDF con filtros
 
**Actor:** Administrador, Tradicional y Esporádico. **Permiso:** `LEER`.
 
**Qué hace el usuario**
 
1. Abre **Reportes** y elige uno de los 5 en `cmbReporte`.
2. El panel de filtros cambia según el reporte: una lista (`cmbFiltro`) o un texto (`txtFiltro`).
3. Pulsa **Generar**, elige dónde guardar el PDF y la aplicación lo abre al terminar.
 
**Los 5 reportes**
 
| # | Reporte | Filtro del usuario | Tablas |
|---|---|---|---|
| 1 | Proveedores por ciudad, con sus productos | Ciudad (o "Todas") | proveedor, ciudad, detalle_producto, producto |
| 2 | Productos que ofrece un proveedor | Proveedor | proveedor, detalle_producto, producto |
| 3 | Proveedores de un producto | Producto | producto, detalle_producto, proveedor, ciudad |
| 4 | Productos por categoría | Categoría (o "Todas") | producto, categoria_producto |
| 5 | Productos con su categoría, buscados por nombre | Texto del nombre + categoría (opcional) | producto, categoria_producto |
 
**Pasos de construcción**
 
1. **Datos:** `ReporteDAO` con un método por reporte. Cada uno recibe los filtros por parámetro y devuelve un `DataTable` ya filtrado.
2. **Datos:** los filtros opcionales se manejan en el SQL con `(:ciudad IS NULL OR pr.ciudad_id = :ciudad)`. Nunca concatenes texto del usuario dentro del SQL.
3. **Negocio:** `ReporteServicio` con `Sesion.Exigir(Permisos.Leer)`; valida los filtros (por ejemplo, que el proveedor o el producto elegido exista) y delega en `ReporteDAO`.
4. **Presentación:** `GeneradorPdf` en la carpeta `Reportes/` con `PDFsharp-MigraDoc`. Un solo generador sirve para los cinco: recibe título, descripción del filtro y la tabla.
5. **Presentación:** cada PDF lleva título, fecha y hora de generación, usuario que lo generó (`Sesion.UsuarioActual`), filtro aplicado, la tabla y el total de filas.
6. **Presentación:** `FrmReportes` con `cmbReporte`, el panel de filtros y `btnGenerar`. Usa un `SaveFileDialog` para el destino y abre el archivo al terminar.
7. **Presentación:** al cambiar `cmbReporte`, muestra u oculta los controles de filtro correspondientes.
 
**Reglas y errores**
 
- Si no hay filas, el PDF se genera igual con total 0 y no falla.
 
**Listo cuando…** los 5 PDF se generan con sus filtros y muestran título, fecha, usuario, filtro y total de filas.
 
---
 
### CU-11 Gestionar usuarios (solo administrador)
 
**Actor:** Administrador. **Permiso:** `GESTIONAR_USUARIOS`. Depende de CU-01 y CU-04.
 
**Qué hace el usuario**
 
1. Abre **Auditoría → Usuarios** y ve la lista con cédula, nombre, ciudad, tipo y si está activo.
2. Para crear: llena cédula, nombre, apellido, fecha de nacimiento, ciudad, tipo de usuario y contraseña, y pulsa **Guardar**.
3. Para editar: selecciona una fila y cambia los datos. Si deja la contraseña vacía, no se cambia.
4. Para dar de baja: selecciona una fila y pulsa **Desactivar**. No existe botón Eliminar.
 
**Pasos de construcción**
 
1. **Modelo:** `Usuario` completo y el DTO `UsuarioVistaDto` (`Cedula`, `NombreCompleto`, `Ciudad`, `Tipo`, `Activo`).
2. **Datos:** `UsuarioDAO` con `ListarVista()`, `ObtenerPorCedula`, `Insertar`, `Actualizar`, `CambiarEstado(cedula, activo)` y `ExisteAdministrador()`. `TipoUsuarioDAO.Listar()`.
3. **Negocio:** `UsuarioServicio.Crear` y `Editar`, con `Sesion.Exigir(Permisos.GestionarUsuarios)`, validan:
   - Cédula obligatoria, única, solo dígitos, entre 6 y 15.
   - Nombre y apellido obligatorios, máximo 60 caracteres.
   - Fecha de nacimiento no futura y edad mínima razonable.
   - Ciudad y tipo de usuario obligatorios.
   - Contraseña obligatoria al crear; vacía al editar significa "no cambiarla".
4. **Negocio:** calcula el hash SHA-256 de la contraseña antes de enviarla al DAO. Nunca se guarda ni se muestra en texto plano.
5. **Negocio:** rechaza crear un segundo administrador. El índice único de Oracle lo respalda, pero la regla se verifica primero aquí.
6. **Negocio:** `UsuarioServicio.Desactivar(cedula)` rechaza desactivar al administrador, cambiarle el tipo o desactivarse a sí mismo (compara con `Sesion.UsuarioActual`).
7. **Presentación:** `FrmUsuarios` con `dgvUsuarios`, `txtCedula`, `txtNombre`, `txtApellido`, `dtpFechaNacimiento`, `cmbCiudad`, `cmbTipoUsuario`, `txtContrasena` (`UseSystemPasswordChar = true`), `chkActivo`, `btnNuevo`, `btnGuardar` y `btnDesactivar`.
8. **Presentación:** en el menú, **Auditoría → Usuarios** se muestra solo si `Sesion.TienePermiso(Permisos.GestionarUsuarios)`.
 
**Reglas y errores**
 
- Solo existe **un** administrador.
- Los usuarios no se eliminan: se desactivan (`activo = 0`). Un usuario inactivo no puede iniciar sesión.
- Cédula duplicada: `DatoDuplicadoException` con mensaje claro.
 
**Listo cuando…** el administrador crea usuarios de los 3 tipos, no puede crear un segundo administrador, no puede desactivarse y el usuario desactivado ya no ingresa.
 
---
 
### CU-12 Consultar bitácora (solo administrador)
 
**Actor:** Administrador. **Permiso:** `VER_BITACORA`. Depende de CU-01 y CU-02, que generan las filas.
 
**Qué hace el usuario**
 
1. Abre **Auditoría → Bitácora** y ve las entradas y salidas de todos los usuarios.
2. Filtra por usuario (o "Todos") y por un rango de fechas, y pulsa **Filtrar**.
3. Las sesiones sin hora de salida aparecen como *"sesión sin cierre"*.
 
**Pasos de construcción**
 
1. **Modelo:** DTO `BitacoraVistaDto` (`Usuario`, `FechaEntrada`, `FechaSalida`).
2. **Datos:** `BitacoraDAO.Listar(cedulaUsuario, desde, hasta)` con JOIN a `usuario` y filtros opcionales por parámetro. Devuelve `List<BitacoraVistaDto>`.
3. **Negocio:** `BitacoraServicio.Listar(...)` llama `Sesion.Exigir(Permisos.VerBitacora)` y valida que `desde` no sea posterior a `hasta`.
4. **Presentación:** `FrmBitacora` con `dgvBitacora`, `cmbUsuario` (con opción "Todos"), `dtpDesde`, `dtpHasta` y `btnFiltrar`.
5. **Presentación:** llena `cmbUsuario` con `UsuarioServicio.ListarVista()`. Al pulsar **Filtrar**, llena la tabla con el resultado.
6. **Presentación:** si `FechaSalida` es nula, muestra *"sesión sin cierre"* en esa celda.
7. **Presentación:** el formulario es **solo lectura**: no tiene Nuevo, Guardar ni Eliminar.
8. **Presentación:** en el menú, **Auditoría → Bitácora** se muestra solo si `Sesion.TienePermiso(Permisos.VerBitacora)`.
 
**Reglas y errores**
 
- La bitácora no se edita ni se borra desde la aplicación.
- Los usuarios con filas en la bitácora no pueden borrarse en Oracle; por eso solo se desactivan.
 
**Listo cuando…** el administrador ve los ingresos y salidas de todos, filtra por usuario y fechas, y los demás tipos de usuario ni siquiera ven la opción.
 
---
 
## 7. Orden de implementación
 
Cada caso de uso depende de los anteriores.
 
| Paso | Casos de uso | Se considera listo cuando… |
|---|---|---|
| 1 | CU-00 (esquema SQL, constantes, excepciones, traductor, `Sesion`, `Conexion`) | La base se crea desde cero y el proyecto compila |
| 2 | CU-04 (`FrmCiudades`, patrón de CRUD) | Ciudades crea, edita y elimina con mensajes claros |
| 3 | CU-05, CU-06 y CU-07 | Cada formulario crea, edita y elimina con mensajes claros |
| 4 | CU-01, CU-02 y CU-03 | El login funciona, el menú cambia según el usuario y la bitácora registra entrada y salida |
| 5 | CU-11 y CU-12 | El administrador crea usuarios de los 3 tipos y consulta la bitácora |
| 6 | CU-08 | Se asignan y quitan productos, y un error a la mitad no deja datos a medias |
| 7 | CU-09 | Las 5 consultas muestran resultados |
| 8 | CU-10 | Los 5 PDF se generan con filtros |
 
## 8. Instrucciones de trabajo
 
- Entrega un caso de uso completo (las cuatro capas) antes de pasar al siguiente.
- Si un caso de uso necesita algo de otro que aún no existe, créalo con la firma del contrato de la sección 5.
- No cambies las firmas del contrato sin avisar.
- Haz un commit pequeño por cada caso de uso terminado.
- Al terminar cada caso, indica qué archivos creaste o modificaste y cómo probarlo.
