using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCitasMedicas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedimientosController : ControllerBase
    {
        private readonly AppDBContext _dbContext;

        public ProcedimientosController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProcedimientos()
        {
            var procedimientos = await _dbContext.Procedimientos
                .Include(p => p.Cita)
                .ToListAsync();
            return Ok(procedimientos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcedimiento(Procedimiento procedimiento)
        {
            if (!_dbContext.Citas.Any(c => c.IdCita == procedimiento.IdCita))
            {
                return BadRequest("Cita no válida.");
            }

            _dbContext.Procedimientos.Add(procedimiento);
            await _dbContext.SaveChangesAsync();
            return Ok(procedimiento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProcedimiento(int id, Procedimiento procedimiento)
        {
            if (id != procedimiento.IdProcedimiento)
            {
                return BadRequest("El ID del procedimiento no coincide.");
            }

            _dbContext.Entry(procedimiento).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(procedimiento);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcedimientoExists(id))
                {
                    return NotFound("Procedimiento no encontrado.");
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcedimiento(int id)
        {
            var procedimiento = await _dbContext.Procedimientos.FindAsync(id);
            if (procedimiento == null)
            {
                return NotFound("Procedimiento no encontrado.");
            }

            _dbContext.Procedimientos.Remove(procedimiento);
            await _dbContext.SaveChangesAsync();
            return Ok($"Procedimiento con ID {id} eliminado correctamente.");
        }

        private bool ProcedimientoExists(int id)
        {
            return _dbContext.Procedimientos.Any(e => e.IdProcedimiento == id);
        }
    }
}