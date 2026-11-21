using CoreWCF;
using TechStore.Servicio.Models;

namespace TechStore.Servicio.Contracts
{
    /// <summary>
    /// Contrato del servicio SOAP. Cada [OperationContract] se publica como
    /// una operación en el WSDL y se invoca enviando un sobre (Envelope) SOAP.
    /// </summary>
    [ServiceContract(Namespace = "http://techstore.com/productos")]
    public interface IProductoService
    {
        // ----- Consultar -----
        [OperationContract] List<Producto> ListarProductos();
        [OperationContract] Producto? ObtenerProducto(int idProducto);
        [OperationContract] List<Producto> BuscarProductos(string texto, string sucursal);

        // ----- Registrar -----
        [OperationContract] RespuestaOperacion RegistrarProducto(Producto producto);

        // ----- Actualizar -----
        [OperationContract] RespuestaOperacion ActualizarProducto(Producto producto);

        // ----- Eliminar -----
        [OperationContract] RespuestaOperacion EliminarProducto(int idProducto);
    }
}
