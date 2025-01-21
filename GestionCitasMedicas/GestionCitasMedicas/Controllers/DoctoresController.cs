using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorDto doctorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctor = new Doctor
            {
                Nombre = doctorDto.Nombre ?? string.Empty,
                Especialidad = doctorDto.Especialidad ?? string.Empty,
                Telefono = doctorDto.Telefono,
                Email = doctorDto.Email,
                Subespecialidad = doctorDto.Subespecialidad ?? string.Empty  // Asignar cadena vacía si Subespecialidad es null
            };

            _dbContext.Doctores.Add(doctor);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctores), new { id = doctor.IdDoctor }, doctor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorUpdateDto doctorUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctor = await _dbContext.Doctores.FindAsync(id);
            if (doctor == null)
            {
                return NotFound("Doctor no encontrado.");
            }

            // Asignar los valores o mantener los actuales si son nulos
            doctor.Nombre = doctorUpdateDto.Nombre ?? doctor.Nombre;
            doctor.Especialidad = doctorUpdateDto.Especialidad ?? doctor.Especialidad;
            doctor.Telefono = doctorUpdateDto.Telefono ?? doctor.Telefono;
            doctor.Email = doctorUpdateDto.Email ?? doctor.Email;
            doctor.Subespecialidad = doctorUpdateDto.Subespecialidad ?? doctor.Subespecialidad;

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

            var citasExistentes = await _dbContext.Citas.AnyAsync(c => c.IdDoctor == id);
            if (citasExistentes)
            {
                return BadRequest("No se puede eliminar el doctor porque tiene citas asociadas.");
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

    public class DoctorDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [StringLength(100, ErrorMessage = "La especialidad no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La especialidad solo puede contener letras y espacios.")]
        public string? Especialidad { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        [RegularExpression(@"^\d{3}\s\d{3}\s\d{4}$", ErrorMessage = "El teléfono no tiene un formato válido.")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "La subespecialidad no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La subespecialidad solo puede contener letras y espacios.")]
        public string? Subespecialidad { get; set; }
    }

    public class DoctorUpdateDto
    {
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        [MinLength(1, ErrorMessage = "El nombre no puede estar vacío.")]
        public string? Nombre { get; set; }

        [StringLength(100, ErrorMessage = "La especialidad no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La especialidad solo puede contener letras y espacios.")]
        public string? Especialidad { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        [RegularExpression(@"^\d{3}\s\d{3}\s\d{4}$", ErrorMessage = "El teléfono no tiene un formato válido.")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "La subespecialidad no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La subespecialidad solo puede contener letras y espacios.")]
        public string? Subespecialidad { get; set; }
    }
}
