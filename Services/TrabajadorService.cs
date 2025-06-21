namespace TrabajadoresPrueba.Services;

using Microsoft.EntityFrameworkCore;
using TrabajadoresPrueba.Data;
using TrabajadoresPrueba.Models;

public class TrabajadorService : ITrabajadorService
{
    private readonly PruebaContext _context;

    public TrabajadorService(PruebaContext context)
    {
        _context = context;
    }

    public async Task<List<TrabajadorDTO>> ObtenerTodosAsync()
    {
        return await _context.TrabajadorDTO
            .FromSqlRaw("EXEC sp_ListarTrabajadores")
            .ToListAsync();
    }
    public async Task<Trabajador?> ObtenerPorIdAsync(int id)
    {
        return await _context.Trabajador.FirstOrDefaultAsync(t => t.Id == id);
    }
    public async Task CrearAsync(Trabajador trabajador)
    {
        _context.Trabajador.Add(trabajador);
        var cambios = await _context.SaveChangesAsync();
    }


    public async Task ActualizarAsync(Trabajador trabajador)
    {
        _context.Trabajador.Update(trabajador);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var trabajador = await _context.Trabajador.FindAsync(id);
        if (trabajador != null)
        {
            _context.Trabajador.Remove(trabajador);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> DocumentoExisteAsync(string dni)
    {
        return await _context.Trabajador.AnyAsync(t => t.NumeroDocumento == dni);
    }
    //Filtrar por género, acá se recilcará el SP para listar
    public async Task<List<TrabajadorDTO>> ObtenerFiltradosAsync(string sexo)
    {
        var query = await _context.TrabajadorDTO
            .FromSqlRaw("EXEC sp_ListarTrabajadores")
            .AsNoTracking()
            .ToListAsync();

        if (!string.IsNullOrEmpty(sexo))
        {
            query = query.Where(t => t.Sexo == sexo).ToList();
        }

        return query;
    }
}
public interface ITrabajadorService
{
    Task<List<TrabajadorDTO>> ObtenerTodosAsync();
    Task<Trabajador?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Trabajador trabajador);
    Task ActualizarAsync(Trabajador trabajador);
    Task EliminarAsync(int id);
    Task<bool> DocumentoExisteAsync(string dni);
    Task<List<TrabajadorDTO>> ObtenerFiltradosAsync(string sexo);
}