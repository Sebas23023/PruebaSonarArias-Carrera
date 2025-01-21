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
                .HasOne<Paciente>()
                .WithMany()
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Cascade);

            // Citas -> Doctores
            modelBuilder.Entity<Cita>()
                .HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(c => c.IdDoctor)
                .OnDelete(DeleteBehavior.Cascade);

            // Procedimientos -> Citas
            modelBuilder.Entity<Procedimiento>()
                .HasOne<Cita>()
                .WithMany()
                .HasForeignKey(p => p.IdCita)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Procedimiento>()
                .Property(p => p.Costo)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Paciente>()
                .Property(p => p.IdPaciente)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Doctor>()
                .Property(d => d.IdDoctor)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Cita>()
                .Property(c => c.IdCita)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Procedimiento>()
                .Property(p => p.IdProcedimiento)
                .ValueGeneratedOnAdd();
        }
    }

    public class Paciente
    {
        [Key]
        public int IdPaciente { get; set; }

        public required string Nombre { get; set; }
        public required DateTime FechaNacimiento { get; set; }
        public required string? Telefono { get; set; }
        public required string? Direccion { get; set; }
    }

    public class Doctor
    {
        [Key]
        public int IdDoctor { get; set; }

        public required string Nombre { get; set; }
        public required string Especialidad { get; set; }
        public required string? Telefono { get; set; }
        public required string? Email { get; set; }
        public required string? Subespecialidad { get; set; }
    }

    public class Cita
    {
        [Key]
        public int IdCita { get; set; }

        public required DateTime Fecha { get; set; }
        public required TimeSpan Hora { get; set; }
        public required string? Motivo { get; set; }

        public required int IdPaciente { get; set; }
        public required int IdDoctor { get; set; }
    }

    public class Procedimiento
    {
        [Key]
        public int IdProcedimiento { get; set; } 

        public required string Descripcion { get; set; }

        public required decimal Costo { get; set; }

        public required int IdCita { get; set; }
    }
}
