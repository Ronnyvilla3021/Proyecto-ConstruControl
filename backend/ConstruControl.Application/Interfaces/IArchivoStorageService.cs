namespace ConstruControl.Application.Interfaces;

public interface IArchivoStorageService
{
    /// <summary>
    /// Guarda un archivo en disco y devuelve la ruta relativa para almacenar en BD.
    /// </summary>
    Task<string> GuardarArchivoAsync(Stream contenido, string nombreOriginal, string carpeta);
}
