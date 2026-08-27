namespace ConstruControl.Application.DTOs.Auth;

public class RegisterRequestDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // "Admin","JefeObra","Bodeguero","Empleado"
}
