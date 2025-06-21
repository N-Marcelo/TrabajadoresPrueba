using System.Collections.Generic;

namespace TrabajadoresPrueba.Models.ViewModels;
public class TrabajadorViewModel
{
    public List<TrabajadorDTO>? ListaTrabajadores { get; set; }
    public Trabajador? NuevoTrabajador { get; set; } // Para creación
    public Trabajador? TrabajadorEditar { get; set; } // Para edición
    public List<Departamento>? Departamentos { get; set; }
    public List<Provincia>? Provincias { get; set; }
    public List<Distrito>? Distritos { get; set; }
}

