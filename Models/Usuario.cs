using System.ComponentModel.DataAnnotations;

namespace PoliclinicoWeb.Models
{
    public class Usuario
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Contraseña { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string TipoUsuario { get; set; } = string.Empty; // Admin, Doctor, Paciente

        public int? IdRelacionado { get; set; } // IdDoctor o IdPaciente según el tipo

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }
    }
}
