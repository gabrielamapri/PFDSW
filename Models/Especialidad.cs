using System.ComponentModel.DataAnnotations;

namespace PoliclinicoWeb.Models
{
    public class Especialidad
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int IdEspecialidad { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
