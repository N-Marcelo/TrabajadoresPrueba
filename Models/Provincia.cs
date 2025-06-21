namespace TrabajadoresPrueba.Models;
public class Provincia
{
    public int Id { get; set; }
    public int? IdDepartamento { get; set; }
    public string? NombreProvincia { get; set; }

    public Departamento? Departamento { get; set; }
    public ICollection<Distrito>? Distritos { get; set; }
    public ICollection<Trabajador>? Trabajadores { get; set; }
}