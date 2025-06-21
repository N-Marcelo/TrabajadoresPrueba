namespace TrabajadoresPrueba.Models;
public class Distrito
{
    public int Id { get; set; }
    public int? IdProvincia { get; set; }
    public string? NombreDistrito { get; set; }

    public Provincia Provincia { get; set; }
    public ICollection<Trabajador> Trabajadores { get; set; }
}