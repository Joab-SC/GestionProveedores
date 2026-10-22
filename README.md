# Gestión de Proveedores

Aplicación de escritorio para administrar **proveedores, productos y categorías**. Incluye control de acceso por tipo de usuario, bitácora de sesiones, consultas SQL y reportes en PDF.

| Componente | Tecnología |
|---|---|
| Lenguaje y framework | C# sobre .NET 10 |
| Interfaz | Windows Forms |
| Base de datos | Oracle Free (imagen `gvenzl/oracle-free`) en Docker |
| Driver de base de datos | `Oracle.ManagedDataAccess.Core` |
| Configuración | Archivo `.env`, leído con `DotNetEnv` |
| Reportes | `PDFsharp-MigraDoc` |

---

## Puesta en marcha

### 1. Configurar las variables

Copia la plantilla y cambia las contraseñas:

```bash
cp Database/.env.example Database/.env
```

| Variable | Para qué sirve |
|---|---|
| `ORACLE_HOST` | Servidor de Oracle (normalmente `localhost`) |
| `ORACLE_PORT` | Puerto (por defecto `1521`) |
| `ORACLE_SERVICE` | Nombre del servicio (`FREEPDB1` en Oracle Free) |
| `ORACLE_PASSWORD` | Contraseña de los usuarios administradores de Oracle (SYS/SYSTEM). Solo la usa Docker |
| `APP_USER` | Usuario de Oracle con el que se conecta la aplicación. Docker lo crea al iniciar |
| `APP_USER_PASSWORD` | Contraseña de ese usuario |

`Database/.env` está en `.gitignore` y no se sube al repositorio.

### 2. Levantar la base de datos

Desde la carpeta `Database`:

```bash
docker compose up -d
```

La primera vez el contenedor tarda un par de minutos en quedar listo. Los datos se guardan en el volumen `oracle-data`.

### 3. Crear las tablas y los datos de prueba

Conéctate como el usuario de la aplicación y ejecuta el script:

```bash
sqlplus <APP_USER>/<APP_USER_PASSWORD>@localhost:1521/FREEPDB1 @01_schema.sql
```

> El script **borra y vuelve a crear** todas las tablas. Si lo ejecutas de nuevo, se pierden los datos que hayas agregado.

### 4. Ejecutar la aplicación

Abre `GestionProveedores.slnx` en Visual Studio y ejecuta el proyecto `GestionProveedores`, o desde la terminal:

```bash
dotnet run --project GestionProveedores
```

### Usuarios de prueba

Para iniciar sesión se usa la **cédula** y la contraseña.

| Cédula | Contraseña | Nombre | Tipo |
|---|---|---|---|
| `1000000001` | `Admin123*` | Admin Sistema | Administrador |
| `1000000002` | `Trad123*` | Laura Gomez | Tradicional |
| `1000000003` | `Espo123*` | Carlos Ruiz | Esporádico |

---

## Arquitectura

La solución está dividida en cuatro proyectos, uno por capa. Cada capa solo conoce a la que tiene debajo.

```
┌──────────────────────────────────────────────┐
│ GestionProveedores          (Presentación)   │  Formularios WinForms y PDF
└──────────────────────┬───────────────────────┘
                       ▼
┌──────────────────────────────────────────────┐
│ Proveedores.Negocio         (Reglas)         │  Servicios, sesión y permisos
└──────────────────────┬───────────────────────┘
                       ▼
┌──────────────────────────────────────────────┐
│ Proveedores.Datos           (Acceso a datos) │  DAOs, SQL y conexión
└──────────────────────┬───────────────────────┘
                       ▼
                 Oracle (Docker)

Proveedores.Modelo: entidades, DTOs, constantes y excepciones (lo usan todas las capas)
```

| Proyecto | Responsabilidad | Lo que no hace |
|---|---|---|
| **GestionProveedores** | Muestra los formularios, recoge lo que escribe el usuario y muestra mensajes. Genera los PDF | No tiene SQL ni reglas de negocio. No conoce la capa de Datos |
| **Proveedores.Negocio** | Valida los datos, aplica las reglas y revisa los permisos antes de cada operación | No usa nada de Oracle |
| **Proveedores.Datos** | Ejecuta el SQL contra Oracle y convierte los errores de Oracle en excepciones propias | No decide reglas de negocio |
| **Proveedores.Modelo** | Define las clases que viajan entre capas | No depende de ningún otro proyecto |

### Estructura de carpetas

```
GestionProveedores.slnx
├── Database/
│   ├── compose.yaml          contenedor de Oracle
│   ├── 01_schema.sql         tablas, datos iniciales y de prueba
│   ├── .env.example          plantilla de variables
│   └── .env                  variables reales (no se sube)
├── GestionProveedores/       Presentación: Frm*.cs, Program.cs, Reportes/GeneradorPdf.cs
├── Proveedores.Negocio/      *Servicio.cs, Sesion.cs, HashContrasena.cs
├── Proveedores.Datos/        *DAO.cs, Conexion.cs, TraductorErroresOracle.cs
└── Proveedores.Modelo/       entidades, *Dto.cs, Permisos.cs, TiposUsuario.cs, *Exception.cs
```

### Recorrido de una operación

Por ejemplo, al guardar una ciudad nueva:

1. `FrmCiudades` llama a `CiudadServicio.Crear(nombre)`.
2. El servicio revisa el permiso con `Sesion.Exigir(Permisos.Crear)` y valida el nombre.
3. Llama a `CiudadDAO.Insertar(...)`, que abre una conexión y ejecuta el `INSERT`.
4. Si Oracle responde con un error (por ejemplo, nombre repetido), el DAO lo convierte en `DatoDuplicadoException`.
5. El servicio cambia el mensaje por uno claro (*"Ya existe una ciudad con ese nombre."*) y el formulario lo muestra.

---

## Componentes principales

### Conexión a la base de datos (`Proveedores.Datos/Conexion.cs`)

Es el único lugar que sabe cómo conectarse a Oracle:

1. La primera vez que se usa, busca un `.env`. Parte de la carpeta del ejecutable y sube carpeta por carpeta, probando `.env` y `Database/.env` en cada una.
2. Carga sus valores como variables de entorno. Si una variable ya existe en el sistema, se respeta la del sistema.
3. Si falta alguna variable, lanza un error que dice cuál falta.
4. Arma la cadena de conexión y devuelve una `OracleConnection` sin abrir.

Al compilar, el proyecto `GestionProveedores` copia `Database/.env` a la carpeta de salida (`bin/...`). Por eso, después de cambiar el `.env`, hay que recompilar.

Cada DAO abre y cierra su propia conexión con `using`. El driver de Oracle reutiliza las conexiones por debajo (pool), así que esto no es costoso.

### Sesión y permisos (`Proveedores.Negocio/Sesion.cs`)

Clase estática que guarda el usuario que inició sesión, sus permisos y el id de su fila en la bitácora.

- `TienePermiso(permiso)` responde si el usuario tiene un permiso. Se usa para mostrar u ocultar opciones del menú y botones.
- `Exigir(permiso)` lanza `PermisoDenegadoException` si no lo tiene. **Todos los métodos de los servicios empiezan con esta llamada.** Esta es la protección real: ocultar botones solo es una comodidad.

### Autenticación (`AutenticacionServicio` y `HashContrasena`)

- Las contraseñas se guardan como hash **SHA-256** (64 caracteres hexadecimales en minúscula), nunca en texto plano.
- Al iniciar sesión se calcula el hash de lo que escribió el usuario y se compara con el guardado. La comparación tarda siempre lo mismo, para no dar pistas por el tiempo de respuesta.
- Si algo falla (la cédula no existe, la contraseña no coincide o el usuario está inactivo), el mensaje es siempre el mismo: *"Cédula o contraseña incorrectos."*
- Después de 3 intentos fallidos la aplicación se cierra.

### Manejo de errores

| Excepción (`Proveedores.Modelo`) | Cuándo ocurre |
|---|---|
| `ReglaNegocioException` | Base de todas. Datos inválidos o una regla que no se cumple |
| `DatoDuplicadoException` | Valor repetido (Oracle `ORA-00001`) |
| `RegistroEnUsoException` | Se intenta borrar algo que otros registros usan (`ORA-02292`) |
| `PermisoDenegadoException` | El usuario no tiene el permiso necesario |

`TraductorErroresOracle` hace la conversión desde los errores de Oracle. Los formularios atrapan `ReglaNegocioException` y muestran su mensaje, así que nunca aparece un código `ORA-xxxxx` en pantalla.

### Reportes PDF (`GestionProveedores/Reportes/GeneradorPdf.cs`)

Un solo generador sirve para todos los reportes. Recibe el título, el filtro aplicado y la tabla de datos. Cada PDF incluye título, fecha y hora, usuario que lo generó, filtro usado, la tabla y el total de filas.

---

## Modelo de datos

| Tabla | Qué guarda | Detalles |
|---|---|---|
| `ciudad` | Ciudades | Nombre único, máx. 80 caracteres |
| `categoria_producto` | Categorías de productos | Nombre y descripción |
| `producto` | Productos | Pertenece a una categoría |
| `proveedor` | Proveedores | Llave: cédula. Pertenece a una ciudad |
| `detalle_producto` | Qué productos ofrece cada proveedor | Par producto–proveedor único. Se borra en cascada con el producto o el proveedor |
| `tipo_usuario` | Tipos de usuario | 1 Administrador, 2 Tradicional, 3 Esporádico |
| `permiso` | Permisos | `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR`, `GESTIONAR_USUARIOS`, `VER_BITACORA` |
| `detalle_tipo_usuario` | Qué permisos tiene cada tipo | Relación tipo–permiso |
| `usuario` | Usuarios de la aplicación | Llave: cédula. Contraseña en hash, campo `activo` (0/1). Un índice único permite **un solo administrador** |
| `bitacora` | Entradas y salidas de sesión | `fecha_salida` vacía si la sesión no se cerró bien |

```
ciudad ──< proveedor ──< detalle_producto >── producto >── categoria_producto
   │
   └────< usuario >── tipo_usuario ──< detalle_tipo_usuario >── permiso
             │
             └──< bitacora
```

### Permisos por tipo de usuario

| Permiso | Administrador | Tradicional | Esporádico |
|---|:---:|:---:|:---:|
| `CREAR` | ✅ | ✅ | |
| `LEER` | ✅ | ✅ | ✅ |
| `ACTUALIZAR` | ✅ | ✅ | |
| `ELIMINAR` | ✅ | ✅ | |
| `GESTIONAR_USUARIOS` | ✅ | | |
| `VER_BITACORA` | ✅ | | |

En resumen: el **Administrador** puede hacer todo, el **Tradicional** gestiona los datos pero no usuarios ni bitácora, y el **Esporádico** solo consulta.

---

## Casos de uso

| CU | Caso de uso | Quién lo usa | Permiso |
|---|---|---|---|
| 01 | Iniciar sesión | Todos | Ninguno |
| 02 | Cerrar sesión | Todos | Ninguno |
| 03 | Menú principal según permisos | Todos | Depende de la opción |
| 04 | Gestionar ciudades | Administrador, Tradicional | `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR` |
| 05 | Gestionar categorías | Administrador, Tradicional | `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR` |
| 06 | Gestionar productos | Administrador, Tradicional | `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR` |
| 07 | Gestionar proveedores | Administrador, Tradicional | `CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR` |
| 08 | Asignar productos a proveedores | Administrador, Tradicional | `CREAR`, `ACTUALIZAR`, `ELIMINAR` |
| 09 | Ejecutar consultas | Todos | `LEER` |
| 10 | Generar reportes en PDF | Todos | `LEER` |
| 11 | Gestionar usuarios | Administrador | `GESTIONAR_USUARIOS` |
| 12 | Consultar bitácora | Administrador | `VER_BITACORA` |

### CU-01 Iniciar sesión

Al abrir la aplicación aparece la ventana de inicio de sesión (`FrmLogin`). El usuario escribe su cédula y su contraseña y pulsa **Ingresar** (o Enter).

- Si los datos son correctos, se cargan sus permisos, se registra la entrada en la bitácora y se abre el menú principal.
- Si no, ve *"Cédula o contraseña incorrectos."*, sin saber cuál de los dos falló.
- Un usuario desactivado no puede entrar.
- Con 3 intentos fallidos la aplicación se cierra.

### CU-02 Cerrar sesión

El usuario elige **Sesión → Cerrar sesión** o cierra la ventana con la X. En los dos casos se guarda la hora de salida en la bitácora.

- Si la aplicación se cierra de forma inesperada, la sesión queda sin hora de salida y la bitácora la muestra como *"sesión sin cierre"*.
- Cerrar sesión dos veces no causa errores.

### CU-03 Menú principal

El menú solo muestra lo que el usuario puede usar. En la barra inferior aparecen su nombre y su tipo de usuario.

| Opción | Se muestra si tiene… |
|---|---|
| Entidades y Transacciones | `CREAR`, `ACTUALIZAR` o `ELIMINAR` |
| Consultas y Reportes | `LEER` |
| Auditoría → Usuarios | `GESTIONAR_USUARIOS` |
| Auditoría → Bitácora | `VER_BITACORA` |
| Sesión → Cerrar sesión | Siempre |

La aplicación siempre pregunta por un permiso, nunca por el tipo de usuario. Así, para cambiar lo que puede hacer un tipo basta con cambiar sus permisos en la base de datos.

### CU-04 a CU-07 Gestionar ciudades, categorías, productos y proveedores

Los cuatro formularios funcionan igual:

- Al abrir se ve la lista de registros.
- **Nuevo** limpia los campos para crear un registro.
- Al seleccionar una fila, sus datos pasan a los campos para editarlos.
- **Guardar** crea el registro si es nuevo o lo actualiza si ya existía.
- **Eliminar** pide confirmación antes de borrar.
- Los botones se habilitan según los permisos del usuario.

Reglas de cada uno:

| Entidad | Campos | Reglas |
|---|---|---|
| **Ciudades** | Nombre | Obligatorio, máx. 80 caracteres, no se repite. No se puede borrar si tiene proveedores o usuarios |
| **Categorías** | Nombre, descripción | Nombre obligatorio (máx. 60), descripción máx. 250. No se puede borrar si tiene productos |
| **Productos** | Nombre, descripción, categoría | Nombre obligatorio (máx. 60), descripción máx. 250, categoría obligatoria. La lista muestra el nombre de la categoría. Al borrar un producto también se borran sus asignaciones a proveedores (se avisa antes) |
| **Proveedores** | Cédula, nombre, apellido, fecha de nacimiento, ciudad | Cédula de 6 a 15 dígitos, no se repite y no se puede cambiar al editar. Nombre y apellido obligatorios (máx. 60). Fecha no futura y edad mínima de 18 años. Ciudad obligatoria. Al borrar un proveedor también se borran sus asignaciones |

### CU-08 Asignar productos a proveedores

Es la **transacción** principal del sistema.

1. El usuario elige un proveedor.
2. Ve dos listas: a la izquierda los productos que el proveedor **no** ofrece y a la derecha los que **sí** ofrece.
3. Con **Agregar** y **Quitar** mueve productos entre las listas. Estos cambios solo existen en pantalla.
4. Al pulsar **Guardar cambios**, se reemplazan todas las asignaciones del proveedor en una sola transacción: se borran las anteriores y se insertan las nuevas.

Si algo falla a mitad del proceso, la transacción se deshace (rollback) y el proveedor queda como estaba. Nunca quedan datos a medias.

### CU-09 Ejecutar consultas

El usuario elige una consulta de la lista, lee su descripción y pulsa **Ejecutar**. El resultado aparece en una tabla.

| # | Consulta | Tipo |
|---|---|---|
| 1 | Proveedores con su ciudad y los productos que ofrecen | JOIN de 4 tablas |
| 2 | Productos por categoría con su número de proveedores | JOIN de 3 tablas con agrupación |
| 3 | Usuarios con su ciudad, tipo y permisos | JOIN de 5 tablas |
| 4 | Proveedores que ofrecen más productos que el promedio | Subconsulta |
| 5 | Productos que ningún proveedor ofrece | Subconsulta con `NOT EXISTS` |

La consulta 5 sale vacía si todos los productos tienen al menos un proveedor.

### CU-10 Generar reportes en PDF

El usuario elige un reporte, completa su filtro, pulsa **Generar** y escoge dónde guardar el archivo. Al terminar, el PDF se abre solo.

| # | Reporte | Filtro |
|---|---|---|
| 1 | Proveedores por ciudad, con sus productos | Ciudad (o todas) |
| 2 | Productos que ofrece un proveedor | Proveedor |
| 3 | Proveedores de un producto | Producto |
| 4 | Productos por categoría | Categoría (o todas) |
| 5 | Búsqueda de productos por nombre | Texto y categoría opcional |

Los filtros se pasan a Oracle como parámetros, nunca pegados al texto del SQL. Si no hay resultados, el PDF se genera igual con total 0.

### CU-11 Gestionar usuarios

Solo el administrador entra aquí. Ve la lista de usuarios con cédula, nombre, ciudad, tipo y estado.

- **Crear:** cédula, nombre, apellido, fecha de nacimiento, ciudad, tipo y contraseña. Todos son obligatorios.
- **Editar:** si la contraseña se deja vacía, se conserva la anterior.
- **Desactivar:** los usuarios nunca se borran, solo se desactivan. Un usuario desactivado no puede iniciar sesión.

Reglas:

- La cédula solo lleva dígitos y no se repite.
- Edad mínima de 18 años y fecha de nacimiento no futura.
- Solo puede existir **un administrador**: no se puede crear otro ni cambiar el tipo del actual.
- El administrador no puede desactivarse ni desactivarse a sí mismo.
- La contraseña se guarda como hash SHA-256.

### CU-12 Consultar bitácora

Solo el administrador entra aquí. Ve las entradas y salidas de sesión y puede filtrar por usuario (o todos) y por rango de fechas.

- Las sesiones sin hora de salida aparecen como *"sesión sin cierre"*.
- La fecha inicial no puede ser posterior a la final.
- Es solo lectura: la bitácora no se edita ni se borra desde la aplicación.
