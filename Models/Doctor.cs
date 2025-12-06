using System.ComponentModel.DataAnnotations;

namespace PoliclinicoWeb.Models
{
    public class Doctor
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int IdDoctor { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(8)]
        public string DNI { get; set; } = string.Empty;

        public int IdEspecialidad { get; set; }

        public string? NombreEspecialidad { get; set; }

        [StringLength(15)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? CMP { get; set; }

        [StringLength(50)]
        public string? RNE { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; }

        // Propiedad calculada
        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
}
