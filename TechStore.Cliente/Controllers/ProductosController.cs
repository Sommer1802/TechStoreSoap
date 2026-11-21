using System.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using TechStore.Cliente.Models;
using TechStore.Cliente.ServicioProductos;

namespace TechStore.Cliente.Controllers
{
    /// <summary>
    /// Controlador MVC que consume el servicio SOAP a través del proxy
    /// ProductoServiceClient (generado desde el WSDL).
    /// </summary>
    public class ProductosController : Controller
    {
        private readonly string _urlServicio;

        public ProductosController(IConfiguration config)
        {
            _urlServicio = config["ServicioSoap:Url"]!;
        }

        // Crea el cliente SOAP, ejecuta la operación y cierra el canal correctamente.
        private async Task<T> LlamarServicio<T>(Func<ProductoServiceClient, Task<T>> operacion)
        {
            var cliente = new ProductoServiceClient(
                ProductoServiceClient.EndpointConfiguration.BasicHttpBinding_IProductoService, _urlServicio);
            try
            {
                var resultado = await operacion(cliente);
                await cliente.CloseAsync();
                return resultado;
            }
            catch
            {
                cliente.Abort();
                throw;
            }
        }

        private IActionResult ErrorConexion(Exception ex)
        {
            ViewBag.Error = ex is EndpointNotFoundException or CommunicationException
                ? $"No se pudo conectar con el servicio SOAP ({_urlServicio}). ¿Está en ejecución?"
                : $"Error al llamar el servicio: {ex.Message}";
            return View("ErrorServicio");
        }

        // GET: /Productos  (ListarProductos / BuscarProductos)
        public async Task<IActionResult> Index(string? texto, string? sucursal)
        {
            try
            {
                var productos = string.IsNullOrWhiteSpace(texto) && string.IsNullOrWhiteSpace(sucursal)
                    ? await LlamarServicio(c => c.ListarProductosAsync())
                    : await LlamarServicio(c => c.BuscarProductosAsync(texto ?? "", sucursal ?? ""));

                ViewBag.Texto = texto;
                ViewBag.Sucursal = sucursal;
                return View(productos);
            }
            catch (Exception ex) { return ErrorConexion(ex); }
        }

        // GET: /Productos/Detalles/5  (ObtenerProducto)
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                var p = await LlamarServicio(c => c.ObtenerProductoAsync(id));
                if (p == null) return NotFound();
                return View(p);
            }
            catch (Exception ex) { return ErrorConexion(ex); }
        }

        // GET: /Productos/Crear
        public IActionResult Crear() => View("Formulario", new ProductoViewModel());

        // POST: /Productos/Crear  (RegistrarProducto)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoViewModel modelo)
        {
            if (!ModelState.IsValid) return View("Formulario", modelo);
            try
            {
                var r = await LlamarServicio(c => c.RegistrarProductoAsync(modelo.AProducto()));
                if (!r.Exito)
                {
                    ModelState.AddModelError("", r.Mensaje);
                    return View("Formulario", modelo);
                }
                TempData["Ok"] = $"{r.Mensaje} (Id {r.IdAfectado})";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { return ErrorConexion(ex); }
        }

        // GET: /Productos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var p = await LlamarServicio(c => c.ObtenerProductoAsync(id));
                if (p == null) return NotFound();
                return View("Formulario", ProductoViewModel.DesdeProducto(p));
            }
            catch (Exception ex) { return ErrorConexion(ex); }
        }

        // POST: /Productos/Editar/5  (ActualizarProducto)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, ProductoViewModel modelo)
        {
            modelo.IdProducto = id;
            if (!ModelState.IsValid) return View("Formulario", modelo);
            try
            {
                var r = await LlamarServicio(c => c.ActualizarProductoAsync(modelo.AProducto()));
                if (!r.Exito)
                {
                    ModelState.AddModelError("", r.Mensaje);
                    return View("Formulario", modelo);
                }
                TempData["Ok"] = r.Mensaje;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { return ErrorConexion(ex); }
        }

        // GET: /Productos/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var p = await LlamarServicio(c => c.ObtenerProductoAsync(id));
                if (p == null) return NotFound();
                return View(p);
            }
            catch (Exception ex) { return ErrorConexion(ex); }
        }

        // POST: /Productos/Eliminar/5  (EliminarProducto)
        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            try
            {
                var r = await LlamarServicio(c => c.EliminarProductoAsync(id));
                TempData[r.Exito ? "Ok" : "Error"] = r.Mensaje;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { return ErrorConexion(ex); }
        }
    }
}
