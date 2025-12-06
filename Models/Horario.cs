using System.ComponentModel.DataAnnotations;

namespace PoliclinicoWeb.Models
{
    public class Horario
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int IdHorario { get; set; }

        public int IdDoctor { get; set; }

        public string? NombreDoctor { get; set; }

        [Required]
        [StringLength(10)]
        public string DiaSemana { get; set; } = string.Empty; // Lunes, Martes, etc.

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public int DuracionCitaMinutos { get; set; } = 30; // Duración de cada cita en minutos

        public bool Activo { get; set; } = true;

        // Propiedad calculada para mostrar el rango de horas
        public string RangoHorario => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
    }
}
