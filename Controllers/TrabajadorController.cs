using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabajadoresPrueba.Data;
using TrabajadoresPrueba.Models;
using TrabajadoresPrueba.Models.ViewModels;
using TrabajadoresPrueba.Services;

namespace TrabajadoresPrueba.Controllers;

[Route("/Trabajador")]
[Controller]
public class TrabajadorController : Controller
{
    private readonly PruebaContext _context;
    private readonly ITrabajadorService _trabajadorService;

    public TrabajadorController(ITrabajadorService trabajadorService, PruebaContext context)
    {
        _trabajadorService = trabajadorService;
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int? mostrarModalEditarId = null)
    {
        var trabajadores = await _trabajadorService.ObtenerTodosAsync();
        var departamentos = await _context.Departamento.ToListAsync();

        if (TempData["MostrarModalEditarId"] != null)
        {
            mostrarModalEditarId = Convert.ToInt32(TempData["MostrarModalEditarId"]);
            ViewData["MostrarModalEditarId"] = mostrarModalEditarId;
        }

        var viewModel = new TrabajadorViewModel
        {
            ListaTrabajadores = trabajadores,
            NuevoTrabajador = new Trabajador(),
            TrabajadorEditar = mostrarModalEditarId.HasValue
            ? await _trabajadorService.ObtenerPorIdAsync(mostrarModalEditarId.Value)
            : new Trabajador(),
            Departamentos = departamentos,
            Provincias = new List<Provincia>(),
            Distritos = new List<Distrito>()
        };
        //Limpiar erorres de documento y evitar contaminación al cambiar de opción si se falla al registrar o editar
        if (TempData["ErrorDocumentoCrear"] != null)
        {
            ModelState.AddModelError("NuevoTrabajador.NumeroDocumento", TempData["ErrorDocumentoCrear"]!.ToString()!);
            ViewData["MostrarModalCrear"] = true;
        }

        if (TempData["ErrorDocumentoEditar"] != null && TempData["MostrarModalEditarId"] != null)
        {
            ModelState.AddModelError("TrabajadorEditar.NumeroDocumento", TempData["ErrorDocumentoEditar"]!.ToString()!);
            ViewData["MostrarModalEditarId"] = TempData["MostrarModalEditarId"];
        }

        return View(viewModel);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearTrabajador(TrabajadorViewModel model)
    {
        var trabajador = model.NuevoTrabajador;

        if (!ModelState.IsValid || await ValidarDocumentoDuplicadoAsync(trabajador.NumeroDocumento!))
        {
            TempData["ErrorDocumentoCrear"] = "Error en el formulario.";
            if (ModelState.IsValid)
            {
                TempData["ErrorDocumentoCrear"] = "Ya existe un trabajador con este DNI.";
            }
            return RedirectToAction(nameof(Index));
        }
        await _trabajadorService.CrearAsync(trabajador);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarTrabajador(TrabajadorViewModel model)
    {
        var trabajador = model.TrabajadorEditar;
        int id = trabajador.Id;

        if (!ModelState.IsValid || await ValidarDocumentoDuplicadoAsync(trabajador.NumeroDocumento, id))
        {
            TempData["MostrarModalEditarId"] = id;
            if (ModelState.IsValid)
            {
                TempData["ErrorDocumentoEditar"] = "Ya existe un trabajador con este DNI.";
            }
            return RedirectToAction(nameof(Index));
        }

        await _trabajadorService.ActualizarAsync(trabajador);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _trabajadorService.EliminarAsync(id);
            return Json(new { success = true, message = "Trabajador eliminado correctamente." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error al eliminar el trabajador: {ex.Message}" });
        }
    }

    [HttpGet("ProvinciasPorDepartamento/{idDepartamento}")]
    public async Task<JsonResult> ProvinciasPorDepartamento(int idDepartamento)
    {
        var provincias = await _context.Provincia
            .Where(p => p.IdDepartamento == idDepartamento)
            .Select(p => new { p.Id, p.NombreProvincia })
            .ToListAsync();

        return Json(provincias);
    }

    [HttpGet("DistritosPorProvincia/{idProvincia}")]
    public async Task<JsonResult> DistritosPorProvincia(int idProvincia)
    {
        var distritos = await _context.Distrito
            .Where(d => d.IdProvincia == idProvincia)
            .Select(d => new { d.Id, d.NombreDistrito })
            .ToListAsync();

        return Json(distritos);
    }
    [HttpGet("FiltrarPorSexo")]
    public async Task<IActionResult> FiltrarPorSexo(string sexo)
    {
        Console.WriteLine($"Filtrando por sexo: {sexo ?? "null"}");
        try
        {
            var trabajadores = await _trabajadorService.ObtenerFiltradosAsync(sexo);

            //La data podrá contener valores nulos, para evitar que la web se denta por errores de vacíos
            var response = trabajadores.Select(t => new
            {
                id = t.Id,
                tipoDocumento = t.TipoDocumento,
                numeroDocumento = t.NumeroDocumento,
                nombres = t.Nombres,
                sexo = t.Sexo,
                nombreDepartamento = t.NombreDepartamento,
                nombreProvincia = t.NombreProvincia,
                nombreDistrito = t.NombreDistrito
            });

            return Json(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Error interno al filtrar trabajadores");
        }
    }

    private async Task<bool> ValidarDocumentoDuplicadoAsync(string numeroDocumento, int? idExistente = null)
    {
        if (string.IsNullOrWhiteSpace(numeroDocumento)) return false;

        var query = _context.Trabajador.Where(t => t.NumeroDocumento == numeroDocumento);

        if (idExistente.HasValue)
        {
            query = query.Where(t => t.Id != idExistente.Value);
        }

        return await query.AnyAsync();
    }
}
