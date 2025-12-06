using Microsoft.EntityFrameworkCore;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets: ajustar/añadir según modelos existentes en el proyecto
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Doctor> Doctores { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<GrupoSanguineo> GrupoSanguineos { get; set; }
        public DbSet<Cita> Citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuraciones adicionales si hacen falta (nombres de tablas, relaciones, etc.)
            modelBuilder.Entity<Paciente>().ToTable("Pacientes");
            modelBuilder.Entity<Doctor>().ToTable("Doctores");
            modelBuilder.Entity<Especialidad>().ToTable("Especialidades");
            modelBuilder.Entity<GrupoSanguineo>().ToTable("GrupoSanguineo");
            modelBuilder.Entity<Cita>().ToTable("Citas");
        }
    }
}
