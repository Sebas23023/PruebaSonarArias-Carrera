using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestionCitasMedicas
{
    public class AppDBContext : DbContext
    {
        protected AppDBContext() { }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Doctor> Doctores { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Procedimiento> Procedimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Citas -> Pacientes
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Cascade);

            // Citas -> Doctores
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Doctor)
                .WithMany(d => d.Citas)
                .HasForeignKey(c => c.IdDoctor)
                .OnDelete(DeleteBehavior.Cascade);

            // Procedimientos -> Citas
            modelBuilder.Entity<Procedimiento>()
                .HasOne(p => p.Cita)
                .WithMany(c => c.Procedimientos)
                .HasForeignKey(p => p.IdCita)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class Paciente
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }

    public class Doctor
    {
        public int IdDoctor { get; set; }
        public string Nombre { get; set; }
        public string Especialidad { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }

    public class Cita
    {
        [Key]
        public int IdCita { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string? Motivo { get; set; }

        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

        public int IdDoctor { get; set; }
        public Doctor Doctor { get; set; }

        public ICollection<Procedimiento> Procedimientos { get; set; } = new List<Procedimiento>();
    }

    public class Procedimiento
    {
        public int IdProcedimiento { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }

        public int IdCita { get; set; }
        public Cita Cita { get; set; }
    }
}
