namespace PoliclinicoWeb.Models
{
    // ViewModel para el proceso de reserva de citas (Carrito)
    public class ReservaCitaViewModel
    {
        public int Paso { get; set; } = 1; // 1: Especialidad, 2: Doctor, 3: Fecha/Hora, 4: Confirmación
        
        // Paso 1: Selección de especialidad
        public int IdEspecialidad { get; set; }
        public string? NombreEspecialidad { get; set; }
        public List<Especialidad>? Especialidades { get; set; }
        
        // Paso 2: Selección de doctor
        public int IdDoctor { get; set; }
        public string? NombreDoctor { get; set; }
        public List<Doctor>? Doctores { get; set; }
        
        // Paso 3: Selección de fecha y hora
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraCita { get; set; }
        public List<DateTime>? FechasDisponibles { get; set; }
        public List<TimeSpan>? HorasDisponibles { get; set; }
        
        // Paso 4: Datos adicionales
        public int IdPaciente { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        
        // Resumen
        public Cita? CitaResumen { get; set; }
    }
}
