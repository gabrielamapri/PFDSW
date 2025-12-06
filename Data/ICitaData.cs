using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public interface ICitaData
    {
        Task<List<Cita>> ListarTodas();
        Task<List<Cita>> ListarPorPaciente(int idPaciente);
        Task<List<Cita>> ListarPorDoctor(int idDoctor);
        Task<List<Cita>> ListarPorFecha(DateTime fecha);
        Task<PaginacionViewModel<Cita>> ListarConFiltros(int pagina, int registrosPorPagina, 
            int? idEspecialidad = null, int? idDoctor = null, DateTime? fechaInicio = null, DateTime? fechaFin = null, string? estado = null);
        Task<Cita?> ObtenerPorId(int idCita);
        Task<bool> Crear(Cita cita);
        Task<bool> Actualizar(Cita cita);
        Task<bool> CambiarEstado(int idCita, string nuevoEstado);
        Task<bool> Eliminar(int idCita);
        Task<bool> VerificarDisponibilidad(int idDoctor, DateTime fecha, TimeSpan hora);
    }
}
