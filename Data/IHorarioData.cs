using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public interface IHorarioData
    {
        Task<List<Horario>> ListarPorDoctor(int idDoctor);
        Task<Horario?> ObtenerPorId(int idHorario);
        Task<bool> Crear(Horario horario);
        Task<bool> Actualizar(Horario horario);
        Task<bool> Eliminar(int idHorario);
        Task<List<TimeSpan>> ObtenerHorasDisponibles(int idDoctor, DateTime fecha);
    }
}
