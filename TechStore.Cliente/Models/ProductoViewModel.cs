using System.ComponentModel.DataAnnotations;
using TechStore.Cliente.ServicioProductos;

namespace TechStore.Cliente.Models
{
    /// <summary>
    /// Modelo de la vista (formulario). Se convierte a/desde la clase Producto
    /// que generó dotnet-svcutil a partir del WSDL.
    /// </summary>
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El código es obligatorio"), StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = "";

        [Required(ErrorMessage = "El nombre es obligatorio"), StringLength(100)]
        public string Nombre { get; set; } = "";

        [StringLength(250)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria"), StringLength(50)]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; } = "";

        [Required, Range(1, 999999999, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Precio { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La sucursal es obligatoria")]
        public string Sucursal { get; set; } = "";

        public static readonly string[] Sucursales =
            { "Barranquilla Centro", "Barranquilla Norte", "Soledad", "Puerto Colombia" };

        public static ProductoViewModel DesdeProducto(Producto p) => new()
        {
            IdProducto = p.IdProducto,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Categoria = p.Categoria,
            Precio = p.Precio,
            Stock = p.Stock,
            Sucursal = p.Sucursal
        };

        public Producto AProducto() => new()
        {
            IdProducto = IdProducto,
            Codigo = Codigo.Trim(),
            Nombre = Nombre.Trim(),
            Descripcion = Descripcion?.Trim() ?? "",
            Categoria = Categoria.Trim(),
            Precio = Precio,
            Stock = Stock,
            Sucursal = Sucursal
        };
    }
}
