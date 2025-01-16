using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCitasMedicas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctoresController : ControllerBase
    {
        private readonly AppDBContext _dbContext;

        public DoctoresController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctores()
        {
            var doctores = await _dbContext.Doctores.ToListAsync();
            return Ok(doctores);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(Doctor doctor)
        {
            _dbContext.Doctores.Add(doctor);
            await _dbContext.SaveChangesAsync();
            return Ok(doctor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, Doctor doctor)
        {
            if (id != doctor.IdDoctor)
            {
                return BadRequest("El ID del doctor no coincide.");
            }

            _dbContext.Entry(doctor).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(doctor);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return NotFound("Doctor no encontrado.");
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _dbContext.Doctores.FindAsync(id);
            if (doctor == null)
            {
                return NotFound("Doctor no encontrado.");
            }

            _dbContext.Doctores.Remove(doctor);
            await _dbContext.SaveChangesAsync();
            return Ok($"Doctor con ID {id} eliminado correctamente.");
        }

        private bool DoctorExists(int id)
        {
            return _dbContext.Doctores.Any(e => e.IdDoctor == id);
        }
    }
}