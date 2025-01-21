using System.ComponentModel.DataAnnotations;
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
            var procedimientos = await _dbContext.Procedimientos.ToListAsync();
            return Ok(procedimientos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcedimiento([FromBody] CreateProcedimientoDto procedimientoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _dbContext.Citas.AnyAsync(c => c.IdCita == procedimientoDTO.IdCita))
            {
                return BadRequest("Cita no válida.");
            }

            var procedimiento = new Procedimiento
            {
                IdCita = procedimientoDTO.IdCita,
                Descripcion = procedimientoDTO.Descripcion,
                Costo = procedimientoDTO.Costo
            };

            _dbContext.Procedimientos.Add(procedimiento);
            await _dbContext.SaveChangesAsync();
            return Ok(procedimiento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProcedimiento(int id, [FromBody] UpdateProcedimientoDto procedimientoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var procedimiento = await _dbContext.Procedimientos.FindAsync(id);
            if (procedimiento == null)
            {
                return NotFound("Procedimiento no encontrado.");
            }

            if (procedimientoDTO.Descripcion != null)
            {
                procedimiento.Descripcion = procedimientoDTO.Descripcion;
            }

            if (procedimientoDTO.Costo.HasValue)
            {
                procedimiento.Costo = procedimientoDTO.Costo.Value;
            }

            if (procedimientoDTO.IdCita.HasValue)
            {
                procedimiento.IdCita = procedimientoDTO.IdCita.Value;
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

    public class CreateProcedimientoDto
    {
        [Required(ErrorMessage = "El ID de la cita es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la cita debe ser un número positivo.")]
        public int IdCita { get; set; }

        [Required(ErrorMessage = "La descripción del procedimiento es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser un valor mayor a cero.")]
        public decimal Costo { get; set; }
    }

    public class UpdateProcedimientoDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la cita debe ser un número positivo.")]
        public int? IdCita { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser un valor mayor a cero.")]
        public decimal? Costo { get; set; }
    }
}
