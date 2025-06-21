using System.ComponentModel.DataAnnotations;

namespace TrabajadoresPrueba.Models;

public class Trabajador
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
    public string? TipoDocumento { get; set; }

    [Required(ErrorMessage = "El número de documento es obligatorio.")]
    public string? NumeroDocumento { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string? Nombres { get; set; }

    [Required(ErrorMessage = "El sexo es obligatorio.")]
    public string? Sexo { get; set; }

    [Required(ErrorMessage = "Seleccione un departamento.")]
    public int? IdDepartamento { get; set; }

    [Required(ErrorMessage = "Seleccione una provincia.")]
    public int? IdProvincia { get; set; }

    [Required(ErrorMessage = "Seleccione un distrito.")]
    public int? IdDistrito { get; set; }

    // Relaciones (opcionalmente no validadas)
    public Departamento? Departamento { get; set; }
    public Provincia? Provincia { get; set; }
    public Distrito? Distrito { get; set; }
}
