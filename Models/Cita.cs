using System.ComponentModel.DataAnnotations;

namespace PoliclinicoWeb.Models
{
    public class Cita
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int IdCita { get; set; }

        public int IdPaciente { get; set; }

        public string? NombrePaciente { get; set; }

        public int IdDoctor { get; set; }

        public string? NombreDoctor { get; set; }

        public string? NombreEspecialidad { get; set; }

        public DateTime FechaCita { get; set; }

        public TimeSpan HoraCita { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Atendida, Cancelada

        [StringLength(500)]
        public string? Motivo { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        public decimal? MontoPagado { get; set; }

        public DateTime FechaRegistro { get; set; }

        // Propiedad calculada
        public string FechaHoraFormateada => $"{FechaCita:dd/MM/yyyy} {HoraCita:hh\\:mm}";
    }
}
