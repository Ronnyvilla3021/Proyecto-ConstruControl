using ConstruControl.Application.DTOs.FotosObra;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FotosObraController : ControllerBase
{
    private readonly IFotoObraRepository _fotoRepository;
    private readonly IArchivoStorageService _storageService;

    public FotosObraController(IFotoObraRepository fotoRepository, IArchivoStorageService storageService)
    {
        _fotoRepository = fotoRepository;
        _storageService = storageService;
    }

    [HttpGet("obra/{obraId}")]
    public async Task<ActionResult<List<FotoObraResponseDto>>> ObtenerPorObra(int obraId)
    {
        var fotos = await _fotoRepository.ObtenerPorObraAsync(obraId);
        return Ok(fotos.Select(MapearADto));
    }

    [HttpPost("obra/{obraId}")]
    [Authorize(Roles = "Admin,JefeObra,Bodeguero")]
    public async Task<ActionResult<FotoObraResponseDto>> Subir(int obraId, IFormFile archivo, [FromForm] string? descripcion)
    {
        if (archivo.Length == 0)
        {
            return BadRequest(new { mensaje = "El archivo esta vacio." });
        }

        await using var stream = archivo.OpenReadStream();
        var ruta = await _storageService.GuardarArchivoAsync(stream, archivo.FileName, "fotos-obra");

        var foto = new FotoObra
        {
            ObraId = obraId,
            RutaArchivo = ruta,
            Descripcion = descripcion
        };

        await _fotoRepository.AgregarAsync(foto);
        await _fotoRepository.GuardarCambiosAsync();

        return Created(string.Empty, MapearADto(foto));
    }

    private static FotoObraResponseDto MapearADto(FotoObra foto) => new()
    {
        Id = foto.Id,
        ObraId = foto.ObraId,
        RutaArchivo = foto.RutaArchivo,
        Descripcion = foto.Descripcion,
        FechaSubida = foto.FechaSubida
    };
}
