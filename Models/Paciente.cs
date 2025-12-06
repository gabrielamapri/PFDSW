using System.ComponentModel.DataAnnotations;

namespace PoliclinicoWeb.Models
{
    public class Paciente
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int IdPaciente { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(8)]
        public string DNI { get; set; } = string.Empty;

        public DateTime FechaNacimiento { get; set; }

        [StringLength(1)]
        public string? Sexo { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(15)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(5)]
        public string? GrupoSanguineo { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; }

        // Propiedad calculada
        public string NombreCompleto => $"{Nombres} {Apellidos}";
        
        // Edad calculada
        public int Edad
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
