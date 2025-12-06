using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public interface IPacienteData
    {
        Task<List<Paciente>> ListarTodos();
        Task<PaginacionViewModel<Paciente>> ListarConPaginacion(int pagina, int registrosPorPagina, string? busqueda = null);
        Task<Paciente?> ObtenerPorId(int idPaciente);
        Task<Paciente?> ObtenerPorDNI(string dni);
        Task<bool> Crear(Paciente paciente);
        Task<bool> Actualizar(Paciente paciente);
        Task<bool> Eliminar(int idPaciente);
    }
}
