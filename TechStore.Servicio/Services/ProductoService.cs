using TechStore.Servicio.Contracts;
using TechStore.Servicio.Data;
using TechStore.Servicio.Models;

namespace TechStore.Servicio.Services
{
    /// <summary>
    /// Implementación del contrato SOAP: valida los datos y delega en el repositorio.
    /// </summary>
    public class ProductoService : IProductoService
    {
        private readonly ProductoRepository _repo;

        public ProductoService(ProductoRepository repo) => _repo = repo;

        public List<Producto> ListarProductos() => _repo.Listar();

        public Producto? ObtenerProducto(int idProducto) => _repo.ObtenerPorId(idProducto);

        public List<Producto> BuscarProductos(string texto, string sucursal) =>
            _repo.Buscar(texto?.Trim() ?? "", sucursal?.Trim() ?? "");

        public RespuestaOperacion RegistrarProducto(Producto producto)
        {
            var error = Validar(producto);
            if (error != null) return Falla(error);
            if (_repo.ExisteCodigo(producto.Codigo))
                return Falla($"Ya existe un producto con el código {producto.Codigo}.");

            int id = _repo.Insertar(producto);
            return new RespuestaOperacion { Exito = true, Mensaje = "Producto registrado correctamente.", IdAfectado = id };
        }

        public RespuestaOperacion ActualizarProducto(Producto producto)
        {
            var error = Validar(producto);
            if (error != null) return Falla(error);
            if (_repo.ExisteCodigo(producto.Codigo, producto.IdProducto))
                return Falla($"Otro producto ya usa el código {producto.Codigo}.");

            return _repo.Actualizar(producto)
                ? new RespuestaOperacion { Exito = true, Mensaje = "Producto actualizado correctamente.", IdAfectado = producto.IdProducto }
                : Falla($"No existe el producto con Id {producto.IdProducto}.");
        }

        public RespuestaOperacion EliminarProducto(int idProducto)
        {
            return _repo.Eliminar(idProducto)
                ? new RespuestaOperacion { Exito = true, Mensaje = "Producto eliminado correctamente.", IdAfectado = idProducto }
                : Falla($"No existe el producto con Id {idProducto}.");
        }

        private static string? Validar(Producto? p)
        {
            if (p == null) return "No se recibió el producto.";
            if (string.IsNullOrWhiteSpace(p.Codigo)) return "El código es obligatorio.";
            if (string.IsNullOrWhiteSpace(p.Nombre)) return "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(p.Categoria)) return "La categoría es obligatoria.";
            if (string.IsNullOrWhiteSpace(p.Sucursal)) return "La sucursal es obligatoria.";
            if (p.Precio <= 0) return "El precio debe ser mayor que cero.";
            if (p.Stock < 0) return "El stock no puede ser negativo.";
            return null;
        }

        private static RespuestaOperacion Falla(string mensaje) =>
            new() { Exito = false, Mensaje = mensaje };
    }
}
