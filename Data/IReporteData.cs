namespace PoliclinicoWeb.Data
{
    public interface IReporteData
    {
        List<ReporteCitasPorEspecialidad> ObtenerCitasPorEspecialidad(string fechaInicio, string fechaFin);
        List<ReporteCitasPorDoctor> ObtenerCitasPorDoctor(string fechaInicio, string fechaFin, string estado);
        List<ReporteCitasPorEstado> ObtenerCitasPorEstado();
        List<ReportePacientesFrecuentes> ObtenerPacientesFrecuentes(int top);
    }

    public class ReporteCitasPorEspecialidad
    {
        public string NombreEspecialidad { get; set; } = string.Empty;
        public int TotalCitas { get; set; }
        public int CitasPendientes { get; set; }
        public int CitasConfirmadas { get; set; }
        public int CitasAtendidas { get; set; }
        public int CitasCanceladas { get; set; }
    }

    public class ReporteCitasPorDoctor
    {
        public string NombreDoctor { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int TotalCitas { get; set; }
        public int CitasPendientes { get; set; }
        public int CitasConfirmadas { get; set; }
        public int CitasAtendidas { get; set; }
        public int CitasCanceladas { get; set; }
    }

    public class ReporteCitasPorEstado
    {
        public string Estado { get; set; } = string.Empty;
        public int Total { get; set; }
    }

    public class ReportePacientesFrecuentes
    {
        public string NombrePaciente { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public int TotalCitas { get; set; }
        public DateTime UltimaCita { get; set; } = DateTime.MinValue;
    }
}
