using ConstruControl.Application.Interfaces;

namespace ConstruControl.Infrastructure.Services;

public class ArchivoStorageService : IArchivoStorageService
{
    private readonly string _rutaBase;

    /// <summary>
    /// rutaBase es la carpeta raiz del proyecto API (ContentRootPath), inyectada
    /// desde Program.cs. Asi Infrastructure no depende directamente de tipos de
    /// ASP.NET Core como IWebHostEnvironment.
    /// </summary>
    public ArchivoStorageService(string rutaBase)
    {
        _rutaBase = Path.Combine(rutaBase, "wwwroot", "uploads");
    }

    public async Task<string> GuardarArchivoAsync(Stream contenido, string nombreOriginal, string carpeta)
    {
        var carpetaCompleta = Path.Combine(_rutaBase, carpeta);
        Directory.CreateDirectory(carpetaCompleta);

        var extension = Path.GetExtension(nombreOriginal);
        var nombreUnico = $"{Guid.NewGuid()}{extension}";
        var rutaCompleta = Path.Combine(carpetaCompleta, nombreUnico);

        await using var archivoDestino = new FileStream(rutaCompleta, FileMode.Create);
        await contenido.CopyToAsync(archivoDestino);

        return Path.Combine("uploads", carpeta, nombreUnico).Replace("\\", "/");
    }
}
