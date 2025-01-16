using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GestionCitasMedicas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly AppDBContext _dbContext;

        public PacientesController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetPacientes()
        {
            var pacientes = await _dbContext.Pacientes.ToListAsync();
            return Ok(pacientes);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaciente(Paciente paciente)
        {
            _dbContext.Pacientes.Add(paciente);
            await _dbContext.SaveChangesAsync();
            return Ok(paciente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaciente(int id, Paciente paciente)
        {
            if (id != paciente.IdPaciente)
            {
                return BadRequest("El ID del paciente no coincide.");
            }

            _dbContext.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(paciente);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacienteExists(id))
                {
                    return NotFound("Paciente no encontrado.");
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            var paciente = await _dbContext.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound("Paciente no encontrado.");
            }

            _dbContext.Pacientes.Remove(paciente);
            await _dbContext.SaveChangesAsync();
            return Ok($"Paciente con ID {id} eliminado correctamente.");
        }

        private bool PacienteExists(int id)
        {
            return _dbContext.Pacientes.Any(e => e.IdPaciente == id);
        }
    }
}