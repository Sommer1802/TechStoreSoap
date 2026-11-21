using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace TechStore.Servicio.Data
{
    /// <summary>
    /// Crea la base de datos y la tabla Producto (con datos de ejemplo) si no existen,
    /// ejecutando el script Database/TechStoreSOAP.sql contra el servidor.
    /// </summary>
    public static class InicializadorBD
    {
        public static void Ejecutar(IConfiguration config, string rutaScript)
        {
            var cadena = new SqlConnectionStringBuilder(config.GetConnectionString("TechStoreDB"))
            {
                InitialCatalog = "master" // la BD aún puede no existir
            };

            var script = File.ReadAllText(rutaScript);
            var lotes = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using var cn = new SqlConnection(cadena.ConnectionString);
            cn.Open();
            foreach (var lote in lotes.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                using var cmd = new SqlCommand(lote, cn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
