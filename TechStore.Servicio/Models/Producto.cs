using System.Runtime.Serialization;

namespace TechStore.Servicio.Models
{
    /// <summary>
    /// Entidad Producto. [DataContract] le indica a WCF/CoreWCF que esta clase
    /// se serializa a XML dentro del mensaje SOAP y aparece descrita en el WSDL (XSD).
    /// </summary>
    [DataContract(Namespace = "http://techstore.com/productos")]
    public class Producto
    {
        [DataMember(Order = 1)] public int IdProducto { get; set; }
        [DataMember(Order = 2)] public string Codigo { get; set; } = "";
        [DataMember(Order = 3)] public string Nombre { get; set; } = "";
        [DataMember(Order = 4)] public string Descripcion { get; set; } = "";
        [DataMember(Order = 5)] public string Categoria { get; set; } = "";
        [DataMember(Order = 6)] public decimal Precio { get; set; }
        [DataMember(Order = 7)] public int Stock { get; set; }
        [DataMember(Order = 8)] public string Sucursal { get; set; } = "";
    }

    /// <summary>
    /// Respuesta estándar para operaciones de escritura (registrar, actualizar, eliminar).
    /// </summary>
    [DataContract(Namespace = "http://techstore.com/productos")]
    public class RespuestaOperacion
    {
        [DataMember(Order = 1)] public bool Exito { get; set; }
        [DataMember(Order = 2)] public string Mensaje { get; set; } = "";
        [DataMember(Order = 3)] public int IdAfectado { get; set; }
    }
}
