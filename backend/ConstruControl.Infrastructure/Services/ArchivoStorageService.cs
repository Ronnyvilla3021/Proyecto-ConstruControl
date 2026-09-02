using ConstruControl.Application.Interfaces;

namespace ConstruControl.Infrastructure.Services;

public class ArchivoStorageService : IArchivoStorageService
{
    private readonly string _rutaBase;

    public ArchivoStorageService()
    {
        // wwwroot/uploads - ya excluido del repo por .gitignore
        _rutaBase = Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads");
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

        // Ruta relativa para guardar en BD (independiente del disco/maquina)
        return Path.Combine("uploads", carpeta, nombreUnico).Replace("\\", "/");
    }
}
