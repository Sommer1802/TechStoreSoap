# TechStore S.A.S — Servicio SOAP de Productos (ASP.NET Core + C#)

Aplicación cliente-servidor que **publica** un servicio web SOAP para gestionar productos (CRUD contra SQL Server)
y una aplicación **cliente** ASP.NET Core MVC que lo **consume**.

```
 Navegador ──HTTP──▶ TechStore.Cliente (MVC, :5090)
                         │  ProductoServiceClient (proxy generado desde el WSDL)
                         ▼  mensajes SOAP/XML (POST)
                     TechStore.Servicio (CoreWCF, :5080)  ── /ProductoService.svc?wsdl
                         │  ProductoService → ProductoRepository (ADO.NET)
                         ▼
                     SQL Server LocalDB — base TechStoreSOAP, tabla Producto
```

## Estructura

| Proyecto / carpeta | Contenido |
|---|---|
| `TechStore.Servicio/Models/Producto.cs` | Entidades `[DataContract]` que viajan en el XML |
| `TechStore.Servicio/Contracts/IProductoService.cs` | Contrato `[ServiceContract]` con 6 `[OperationContract]` |
| `TechStore.Servicio/Services/ProductoService.cs` | Implementación y validaciones de negocio |
| `TechStore.Servicio/Data/ProductoRepository.cs` | CRUD con `SqlConnection`/`SqlCommand` parametrizados |
| `TechStore.Servicio/Database/TechStoreSOAP.sql` | Script de BD (se ejecuta solo al iniciar) |
| `TechStore.Servicio/Program.cs` | Publicación del endpoint SOAP y del WSDL |
| `TechStore.Cliente/Connected Services/TechStore.Cliente.ServicioProductos/Reference.cs` | Proxy **generado** por Visual Studio desde el WSDL |
| `TechStore.Cliente/Controllers/ProductosController.cs` | Acciones MVC que llaman al servicio SOAP |
| `TechStore.Cliente/Views/Productos/*` | Listado + búsqueda, registrar/editar, detalle, eliminar |

## Operaciones del servicio

| Operación SOAP | CRUD | Uso en el cliente |
|---|---|---|
| `ListarProductos()` | Consultar | Listado principal |
| `BuscarProductos(texto, sucursal)` | Consultar | Filtro por texto y sucursal |
| `ObtenerProducto(id)` | Consultar | Detalle / cargar formulario |
| `RegistrarProducto(producto)` | Registrar | Formulario "Nuevo producto" |
| `ActualizarProducto(producto)` | Actualizar | Formulario "Editar" |
| `EliminarProducto(id)` | Eliminar | Confirmación de eliminación |

## Requisitos
- .NET SDK 8 o superior (Visual Studio 2022/2026)
- SQL Server LocalDB (`(localdb)\MSSQLLocalDB`, viene con Visual Studio). Si usa otro servidor,
  cambie la cadena `TechStoreDB` en `TechStore.Servicio/appsettings.json`.

## Cómo ejecutar

**Visual Studio:** abrir `TechStoreSoap.sln` → clic derecho en la solución → *Configurar proyectos de inicio* →
*Varios proyectos de inicio* → marcar **TechStore.Servicio** y **TechStore.Cliente** como *Iniciar* (el servicio primero) → F5.

**Consola:** en dos terminales:
```
cd TechStore.Servicio && dotnet run
cd TechStore.Cliente  && dotnet run
```
- Servicio / WSDL: http://localhost:5080/ProductoService.svc?wsdl
- Cliente: http://localhost:5090

## Cómo se generó el cliente a partir del WSDL
Con el servicio en ejecución, en Visual Studio:
1. Clic derecho en `TechStore.Cliente` → *Agregar* → *Referencia de servicio* → *Servicio web de WCF*.
2. URI: `http://localhost:5080/ProductoService.svc?wsdl` → *Ir*.
3. Espacio de nombres: `TechStore.Cliente.ServicioProductos`; tipo de colección `System.Array`; acceso *Público* → *Finalizar*.

El resultado queda en `Connected Services/TechStore.Cliente.ServicioProductos` (`ConnectedService.json` + `Reference.cs`).

## Puntos clave para la sustentación
1. **Servidor:** CoreWCF es la versión de WCF para ASP.NET Core. `AddServiceEndpoint<ProductoService, IProductoService>(new BasicHttpBinding(), "/ProductoService.svc")` define el endpoint (Address-Binding-Contract) y `HttpGetEnabled = true` publica el WSDL.
2. **`[DataContract]` / `[DataMember]`**: indican qué clases y propiedades se serializan a XML.
3. **Cliente:** el proxy `ProductoServiceClient` convierte cada llamada (`RegistrarProductoAsync`) en un sobre SOAP y deserializa la respuesta. El canal se cierra con `CloseAsync()` o `Abort()` si hay error.
4. **Seguridad de datos:** consultas parametrizadas (`@id`, `@Codigo`) evitan inyección SQL; validaciones en el cliente (DataAnnotations) y en el servidor (`ProductoService.Validar`, código único).