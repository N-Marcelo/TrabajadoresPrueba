namespace TrabajadoresPrueba.Models;
public class TrabajadorDTO
{
    public int Id { get; set; }
    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Nombres { get; set; }
    public string? Sexo { get; set; }

    public int IdDepartamento { get; set; }
    public int IdProvincia { get; set; }
    public int IdDistrito { get; set; }

    //Nombres para la lista en el get
    public string? NombreDepartamento { get; set; }
    public string? NombreProvincia { get; set; }
    public string? NombreDistrito { get; set; }    
}