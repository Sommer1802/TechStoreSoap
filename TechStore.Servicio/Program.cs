using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using TechStore.Servicio.Contracts;
using TechStore.Servicio.Data;
using TechStore.Servicio.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Registrar CoreWCF (servidor SOAP) y la generación de metadatos (WSDL)
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

// 2. Registrar acceso a datos y el servicio
builder.Services.AddSingleton<ProductoRepository>();
builder.Services.AddTransient<ProductoService>();

var app = builder.Build();

// 3. Crear la base de datos si no existe
InicializadorBD.Ejecutar(app.Configuration,
    Path.Combine(app.Environment.ContentRootPath, "Database", "TechStoreSOAP.sql"));

// 4. Publicar el endpoint SOAP
app.UseServiceModel(sb =>
{
    sb.AddService<ProductoService>(opciones =>
    {
        opciones.DebugBehavior.IncludeExceptionDetailInFaults = app.Environment.IsDevelopment();
    });

    // BasicHttpBinding = SOAP 1.1 sobre HTTP (máxima compatibilidad)
    sb.AddServiceEndpoint<ProductoService, IProductoService>(
        new BasicHttpBinding(), "/ProductoService.svc");

    // Habilita el WSDL en  /ProductoService.svc?wsdl
    var metadatos = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    metadatos.HttpGetEnabled = true;
});

app.MapGet("/", () => Results.Content("""
    <h2>TechStore S.A.S - Servicio SOAP de Productos</h2>
    <p>Endpoint: <code>/ProductoService.svc</code></p>
    <p>WSDL: <a href="/ProductoService.svc?wsdl">/ProductoService.svc?wsdl</a></p>
    """, "text/html; charset=utf-8"));

app.Run();

