using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCitasMedicas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly AppDBContext _dbContext;

        public CitasController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCitas()
        {
            var citas = await _dbContext.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .Include(c => c.Procedimientos)
                .ToListAsync();
            return Ok(citas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCita(Cita cita)
        {
            if (!_dbContext.Pacientes.Any(p => p.IdPaciente == cita.IdPaciente) ||
                !_dbContext.Doctores.Any(d => d.IdDoctor == cita.IdDoctor))
            {
                return BadRequest("Paciente o Doctor no válido.");
            }

            _dbContext.Citas.Add(cita);
            await _dbContext.SaveChangesAsync();
            return Ok(cita);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCita(int id, Cita cita)
        {
            if (id != cita.IdCita)
            {
                return BadRequest("El ID de la cita no coincide.");
            }

            _dbContext.Entry(cita).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(cita);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitaExists(id))
                {
                    return NotFound("Cita no encontrada.");
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCita(int id)
        {
            var cita = await _dbContext.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound("Cita no encontrada.");
            }

            _dbContext.Citas.Remove(cita);
            await _dbContext.SaveChangesAsync();
            return Ok($"Cita con ID {id} eliminada correctamente.");
        }

        private bool CitaExists(int id)
        {
            return _dbContext.Citas.Any(e => e.IdCita == id);
        }
    }
}