using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> CreatePaciente([FromBody] PacienteDto pacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paciente = new Paciente
            {
                Nombre = pacienteDto.Nombre ?? string.Empty,
                FechaNacimiento = pacienteDto.FechaNacimiento,
                Telefono = pacienteDto.Telefono,
                Direccion = pacienteDto.Direccion
            };

            _dbContext.Pacientes.Add(paciente);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPacientes), new { id = paciente.IdPaciente }, paciente);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDto pacienteUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paciente = await _dbContext.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound("Paciente no encontrado.");
            }

            paciente.Nombre = pacienteUpdateDto.Nombre ?? paciente.Nombre;
            paciente.FechaNacimiento = pacienteUpdateDto.FechaNacimiento ?? paciente.FechaNacimiento;
            paciente.Telefono = pacienteUpdateDto.Telefono ?? paciente.Telefono;
            paciente.Direccion = pacienteUpdateDto.Direccion ?? paciente.Direccion;

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

            var citasAsociadas = await _dbContext.Citas.AnyAsync(c => c.IdPaciente == id);
            if (citasAsociadas)
            {
                return BadRequest("No se puede eliminar al paciente, ya que tiene citas asociadas.");
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

    public class PacienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "La fecha de nacimiento debe tener el formato válido (yyyy-mm-dd).")]
        [Range(typeof(DateTime), "1900-01-01", "2025-12-31", ErrorMessage = "La fecha de nacimiento debe ser válida entre 1900 y 2025.")]
        public DateTime FechaNacimiento { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        [RegularExpression(@"^\d{3}\s\d{3}\s\d{4}$", ErrorMessage = "El teléfono debe tener el formato 000 000 0000.")]
        public string? Telefono { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.'-]+$", ErrorMessage = "La dirección solo puede contener letras, números, espacios y los siguientes caracteres: , . ' -.")]
        public string? Direccion { get; set; }
    }

    public class PacienteUpdateDto
    {
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        [MinLength(1, ErrorMessage = "El nombre no puede estar vacío.")]
        public string? Nombre { get; set; }

        [DataType(DataType.Date, ErrorMessage = "La fecha de nacimiento debe tener el formato válido (yyyy-mm-dd).")]
        [Range(typeof(DateTime), "1900-01-01", "2025-12-31", ErrorMessage = "La fecha de nacimiento debe ser válida entre 1900 y 2025.")]
        public DateTime? FechaNacimiento { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        [RegularExpression(@"^\d{3}\s\d{3}\s\d{4}$", ErrorMessage = "El teléfono debe tener el formato 000 000 0000.")]
        public string? Telefono { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.'-]+$", ErrorMessage = "La dirección solo puede contener letras, números, espacios y los siguientes caracteres: , . ' -.")]
        public string? Direccion { get; set; }
    }
}
