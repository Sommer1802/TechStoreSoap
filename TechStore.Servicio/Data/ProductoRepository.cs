using Microsoft.Data.SqlClient;
using TechStore.Servicio.Models;

namespace TechStore.Servicio.Data
{
    /// <summary>
    /// Acceso a datos con ADO.NET (SqlConnection / SqlCommand) y consultas parametrizadas.
    /// </summary>
    public class ProductoRepository
    {
        private readonly string _cadena;

        public ProductoRepository(IConfiguration config)
        {
            _cadena = config.GetConnectionString("TechStoreDB")
                      ?? throw new InvalidOperationException("Falta la cadena de conexión TechStoreDB");
        }

        private const string Columnas =
            "IdProducto, Codigo, Nombre, Descripcion, Categoria, Precio, Stock, Sucursal";

        public List<Producto> Listar()
        {
            using var cn = new SqlConnection(_cadena);
            using var cmd = new SqlCommand($"SELECT {Columnas} FROM Producto ORDER BY IdProducto", cn);
            cn.Open();
            return Leer(cmd);
        }

        public Producto? ObtenerPorId(int id)
        {
            using var cn = new SqlConnection(_cadena);
            using var cmd = new SqlCommand($"SELECT {Columnas} FROM Producto WHERE IdProducto = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cn.Open();
            return Leer(cmd).FirstOrDefault();
        }

        public List<Producto> Buscar(string texto, string sucursal)
        {
            using var cn = new SqlConnection(_cadena);
            using var cmd = new SqlCommand($@"
                SELECT {Columnas} FROM Producto
                WHERE (@texto = '' OR Nombre LIKE '%' + @texto + '%'
                                   OR Codigo LIKE '%' + @texto + '%'
                                   OR Categoria LIKE '%' + @texto + '%')
                  AND (@sucursal = '' OR Sucursal = @sucursal)
                ORDER BY IdProducto", cn);
            cmd.Parameters.AddWithValue("@texto", texto ?? "");
            cmd.Parameters.AddWithValue("@sucursal", sucursal ?? "");
            cn.Open();
            return Leer(cmd);
        }

        public bool ExisteCodigo(string codigo, int excluirId = 0)
        {
            using var cn = new SqlConnection(_cadena);
            using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Producto WHERE Codigo = @codigo AND IdProducto <> @id", cn);
            cmd.Parameters.AddWithValue("@codigo", codigo);
            cmd.Parameters.AddWithValue("@id", excluirId);
            cn.Open();
            return (int)cmd.ExecuteScalar()! > 0;
        }

        public int Insertar(Producto p)
        {
            using var cn = new SqlConnection(_cadena);
            using var cmd = new SqlCommand(@"
                INSERT INTO Producto (Codigo, Nombre, Descripcion, Categoria, Precio, Stock, Sucursal)
                OUTPUT INSERTED.IdProducto
                VALUES (@Codigo, @Nombre, @Descripcion, @Categoria, @Precio, @Stock, @Sucursal)", cn);
            AgregarParametros(cmd, p);
            cn.Open();
            return (int)cmd.ExecuteScalar()!;
        }

        public bool Actualizar(Producto p)
        {
            using var cn = new SqlConnection(_cadena);
            using var cmd = new SqlCommand(@"
                UPDATE Producto SET Codigo = @Codigo, Nombre = @Nombre, Descripcion = @Descripcion,
                       Categoria = @Categoria, Precio = @Precio, Stock = @Stock, Sucursal = @Sucursal
                WHERE IdProducto = @IdProducto", cn);
            AgregarParametros(cmd, p);
            cmd.Parameters.AddWithValue("@IdProducto", p.IdProducto);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Eliminar(int id)
        {
            using var cn = new SqlConnection(_cadena);
            using var cmd = new SqlCommand("DELETE FROM Producto WHERE IdProducto = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        private static void AgregarParametros(SqlCommand cmd, Producto p)
        {
            cmd.Parameters.AddWithValue("@Codigo", p.Codigo);
            cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion ?? "");
            cmd.Parameters.AddWithValue("@Categoria", p.Categoria);
            cmd.Parameters.AddWithValue("@Precio", p.Precio);
            cmd.Parameters.AddWithValue("@Stock", p.Stock);
            cmd.Parameters.AddWithValue("@Sucursal", p.Sucursal);
        }

        private static List<Producto> Leer(SqlCommand cmd)
        {
            var lista = new List<Producto>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Producto
                {
                    IdProducto = dr.GetInt32(0),
                    Codigo = dr.GetString(1),
                    Nombre = dr.GetString(2),
                    Descripcion = dr.GetString(3),
                    Categoria = dr.GetString(4),
                    Precio = dr.GetDecimal(5),
                    Stock = dr.GetInt32(6),
                    Sucursal = dr.GetString(7)
                });
            }
            return lista;
        }
    }
}
