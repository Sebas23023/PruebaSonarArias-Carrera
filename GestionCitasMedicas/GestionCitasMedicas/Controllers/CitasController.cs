using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
            var citas = await _dbContext.Citas.ToListAsync();
            return Ok(citas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCita([FromBody] CreateCitaDto citaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _dbContext.Pacientes.AnyAsync(p => p.IdPaciente == citaDTO.IdPaciente) ||
                !await _dbContext.Doctores.AnyAsync(d => d.IdDoctor == citaDTO.IdDoctor))
            {
                return BadRequest("Paciente o Doctor no válido.");
            }

            if (!TimeSpan.TryParse(citaDTO.Hora, new CultureInfo("es-ES"), out var hora)) // Se especifica el CultureInfo correcto
            {
                return BadRequest("La hora debe tener el formato válido (hh:mm).");
            }

            var cita = new Cita
            {
                IdPaciente = citaDTO.IdPaciente,
                IdDoctor = citaDTO.IdDoctor,
                Fecha = citaDTO.Fecha,
                Hora = hora,
                Motivo = citaDTO.Motivo
            };

            _dbContext.Citas.Add(cita);
            await _dbContext.SaveChangesAsync();
            return Ok(cita);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCita(int id, [FromBody] UpdateCitaDto citaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cita = await _dbContext.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound("Cita no encontrada.");
            }

            if (citaDTO.Fecha.HasValue)
            {
                cita.Fecha = citaDTO.Fecha.Value;
            }

            if (!string.IsNullOrEmpty(citaDTO.Hora))
            {
                if (!TimeSpan.TryParse(citaDTO.Hora, new CultureInfo("es-ES"), out var hora)) // Se especifica el CultureInfo correcto
                {
                    return BadRequest("La hora debe tener el formato válido (hh:mm).");
                }
                cita.Hora = hora;
            }

            if (!string.IsNullOrEmpty(citaDTO.Motivo))
            {
                cita.Motivo = citaDTO.Motivo;
            }

            if (citaDTO.IdPaciente.HasValue && citaDTO.IdPaciente.Value > 0)
            {
                cita.IdPaciente = citaDTO.IdPaciente.Value;
            }

            if (citaDTO.IdDoctor.HasValue && citaDTO.IdDoctor.Value > 0)
            {
                cita.IdDoctor = citaDTO.IdDoctor.Value;
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

            var tieneProcedimientos = await _dbContext.Procedimientos.AnyAsync(p => p.IdCita == id);
            if (tieneProcedimientos)
            {
                return BadRequest("No se puede eliminar la cita, ya que tiene procedimientos asociados.");
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

    public class CreateCitaDto
    {
        [Required(ErrorMessage = "La fecha de la cita es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "La fecha debe tener un formato válido (yyyy-mm-dd).")]
        [Range(typeof(DateTime), "2025-01-01", "2025-12-31", ErrorMessage = "La fecha debe estar en el rango del 2025.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora de la cita es obligatoria.")]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):([0-5][0-9])$", ErrorMessage = "La hora debe tener el formato válido (hh:mm).")]
        public string? Hora { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(250, ErrorMessage = "El motivo no puede superar los 250 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.'-]+$", ErrorMessage = "El motivo solo puede contener letras, números y los caracteres: , . ' -")]
        public string? Motivo { get; set; }

        [Required(ErrorMessage = "El ID del paciente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del paciente debe ser un número positivo.")]
        public int IdPaciente { get; set; }

        [Required(ErrorMessage = "El ID del doctor es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del doctor debe ser un número positivo.")]
        public int IdDoctor { get; set; }
    }

    public class UpdateCitaDto
    {
        [DataType(DataType.Date, ErrorMessage = "La fecha debe tener un formato válido (yyyy-mm-dd).")]
        [Range(typeof(DateTime), "2025-01-01", "2025-12-31", ErrorMessage = "La fecha debe estar en el rango del 2025.")]
        public DateTime? Fecha { get; set; }

        [RegularExpression(@"^([01]?[0-9]|2[0-3]):([0-5][0-9])$", ErrorMessage = "La hora debe tener el formato válido (hh:mm).")]
        public string? Hora { get; set; }

        [StringLength(250, ErrorMessage = "El motivo no puede superar los 250 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.'-]+$", ErrorMessage = "El motivo solo puede contener letras, números y los caracteres: , . ' -")]
        public string? Motivo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID del paciente debe ser un número positivo.")]
        public int? IdPaciente { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID del doctor debe ser un número positivo.")]
        public int? IdDoctor { get; set; }
    }
}
