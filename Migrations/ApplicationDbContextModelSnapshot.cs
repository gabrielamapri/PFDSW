using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PoliclinicoWeb.Data;

#nullable disable

namespace PoliclinicoWeb.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13");

            modelBuilder.Entity("PoliclinicoWeb.Models.Especialidad", b =>
                {
                    b.Property<int>("IdEspecialidad").ValueGeneratedOnAdd().HasColumnType("int");

                    b.Property<string>("Nombre").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");

                    b.Property<string>("Descripcion").HasMaxLength(500).HasColumnType("nvarchar(500)");

                    b.Property<bool>("Activo").HasColumnType("bit").HasDefaultValue(true);

                    b.HasKey("IdEspecialidad");

                    b.ToTable("Especialidades");
                });

            modelBuilder.Entity("PoliclinicoWeb.Models.GrupoSanguineo", b =>
                {
                    b.Property<int>("IdGrupo").ValueGeneratedOnAdd().HasColumnType("int");

                    b.Property<string>("Codigo").IsRequired().HasMaxLength(5).HasColumnType("nvarchar(5)");

                    b.Property<string>("Nombre").HasMaxLength(50).HasColumnType("nvarchar(50)");

                    b.Property<bool>("Activo").HasColumnType("bit").HasDefaultValue(true);

                    b.HasKey("IdGrupo");

                    b.ToTable("GrupoSanguineo");
                });

            modelBuilder.Entity("PoliclinicoWeb.Models.Doctor", b =>
                {
                    b.Property<int>("IdDoctor").ValueGeneratedOnAdd().HasColumnType("int");

                    b.Property<string>("Nombres").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");

                    b.Property<string>("Apellidos").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");

                    b.Property<string>("DNI").IsRequired().HasMaxLength(8).HasColumnType("nvarchar(8)");

                    b.Property<int>("IdEspecialidad").HasColumnType("int");

                    b.Property<string>("Telefono").HasMaxLength(15).HasColumnType("nvarchar(15)");

                    b.Property<string>("Email").HasMaxLength(100).HasColumnType("nvarchar(100)");

                    b.Property<string>("CMP").HasMaxLength(50).HasColumnType("nvarchar(50)");

                    b.Property<string>("RNE").HasMaxLength(50).HasColumnType("nvarchar(50)");

                    b.Property<string>("Direccion").HasMaxLength(200).HasColumnType("nvarchar(200)");

                    b.Property<bool>("Activo").HasColumnType("bit").HasDefaultValue(true);

                    b.Property<DateTime>("FechaRegistro").HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");

                    b.HasKey("IdDoctor");

                    b.HasIndex("DNI").IsUnique();

                    b.HasIndex("IdEspecialidad");

                    b.ToTable("Doctores");
                });

            modelBuilder.Entity("PoliclinicoWeb.Models.Paciente", b =>
                {
                    b.Property<int>("IdPaciente").ValueGeneratedOnAdd().HasColumnType("int");

                    b.Property<string>("Nombres").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");

                    b.Property<string>("Apellidos").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");

                    b.Property<string>("DNI").IsRequired().HasMaxLength(8).HasColumnType("nvarchar(8)");

                    b.Property<DateTime>("FechaNacimiento").HasColumnType("date");

                    b.Property<string>("Sexo").HasMaxLength(1).HasColumnType("nvarchar(1)");

                    b.Property<string>("Direccion").HasMaxLength(200).HasColumnType("nvarchar(200)");

                    b.Property<string>("Telefono").HasMaxLength(15).HasColumnType("nvarchar(15)");

                    b.Property<string>("Email").HasMaxLength(100).HasColumnType("nvarchar(100)");

                    b.Property<string>("GrupoSanguineo").HasMaxLength(5).HasColumnType("nvarchar(5)");

                    b.Property<bool>("Activo").HasColumnType("bit").HasDefaultValue(true);

                    b.Property<DateTime>("FechaRegistro").HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");

                    b.HasKey("IdPaciente");

                    b.HasIndex("DNI").IsUnique();

                    b.ToTable("Pacientes");
                });

            modelBuilder.Entity("PoliclinicoWeb.Models.Horario", b =>
                {
                    b.Property<int>("IdHorario").ValueGeneratedOnAdd().HasColumnType("int");

                    b.Property<int>("IdDoctor").HasColumnType("int");

                    b.Property<string>("DiaSemana").IsRequired().HasMaxLength(10).HasColumnType("nvarchar(10)");

                    b.Property<TimeSpan>("HoraInicio").HasColumnType("time");

                    b.Property<TimeSpan>("HoraFin").HasColumnType("time");

                    b.Property<int>("DuracionCitaMinutos").HasColumnType("int").HasDefaultValue(30);

                    b.Property<bool>("Activo").HasColumnType("bit").HasDefaultValue(true);

                    b.HasKey("IdHorario");

                    b.HasIndex("IdDoctor");

                    b.ToTable("Horarios");
                });

            modelBuilder.Entity("PoliclinicoWeb.Models.Cita", b =>
                {
                    b.Property<int>("IdCita").ValueGeneratedOnAdd().HasColumnType("int");

                    b.Property<int>("IdPaciente").HasColumnType("int");

                    b.Property<int>("IdDoctor").HasColumnType("int");

                    b.Property<DateTime>("FechaCita").HasColumnType("date");

                    b.Property<TimeSpan>("HoraCita").HasColumnType("time");

                    b.Property<string>("Estado").IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)").HasDefaultValue("Pendiente");

                    b.Property<string>("Motivo").HasMaxLength(500).HasColumnType("nvarchar(500)");

                    b.Property<string>("Observaciones").HasMaxLength(1000).HasColumnType("nvarchar(1000)");

                    b.Property<decimal?>("MontoPagado").HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("FechaRegistro").HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");

                    b.HasKey("IdCita");

                    b.HasIndex("IdPaciente");

                    b.HasIndex("IdDoctor");

                    b.ToTable("Citas");
                });

            modelBuilder.Entity("PoliclinicoWeb.Models.Usuario", b =>
                {
                    b.Property<int>("IdUsuario").ValueGeneratedOnAdd().HasColumnType("int");

                    b.Property<string>("NombreUsuario").IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");

                    b.Property<string>("Contraseña").IsRequired().HasMaxLength(255).HasColumnType("nvarchar(255)");

                    b.Property<string>("TipoUsuario").IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");

                    b.Property<int?>("IdRelacionado").HasColumnType("int");

                    b.Property<bool>("Activo").HasColumnType("bit").HasDefaultValue(true);

                    b.Property<DateTime>("FechaCreacion").HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");

                    b.HasKey("IdUsuario");

                    b.HasIndex("NombreUsuario").IsUnique();

                    b.ToTable("Usuarios");
                });

#pragma warning restore 612, 618
        }
    }
}
