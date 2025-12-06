using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public interface IEspecialidadData
    {
        Task<List<Especialidad>> ListarTodas();
        Task<List<Especialidad>> ListarActivas();
        Task<Especialidad?> ObtenerPorId(int idEspecialidad);
        Task<bool> Crear(Especialidad especialidad);
        Task<bool> Actualizar(Especialidad especialidad);
        Task<bool> Eliminar(int idEspecialidad);
    }
}
