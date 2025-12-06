using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public interface IDoctorData
    {
        Task<List<Doctor>> ListarTodos();
        Task<List<Doctor>> ListarActivos();
        Task<List<Doctor>> ListarPorEspecialidad(int idEspecialidad);
        Task<Doctor?> ObtenerPorId(int idDoctor);
        Task<bool> Crear(Doctor doctor);
        Task<bool> Actualizar(Doctor doctor);
        Task<bool> Eliminar(int idDoctor);
    }
}
