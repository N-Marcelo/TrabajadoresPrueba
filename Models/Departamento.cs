namespace TrabajadoresPrueba.Models;
public class Departamento
{
    public int Id { get; set; }
    public string? NombreDepartamento { get; set; }

    public ICollection<Provincia> Provincias { get; set; }
    public ICollection<Trabajador> Trabajadores { get; set; }
}