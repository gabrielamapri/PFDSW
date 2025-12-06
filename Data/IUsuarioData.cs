using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public interface IUsuarioData
    {
        Task<Usuario?> ValidarUsuario(string nombreUsuario, string contraseña);
        Task<Usuario?> ObtenerPorId(int idUsuario);
        Task<List<Usuario>> ListarTodos();
        Task<bool> Crear(Usuario usuario);
        Task<bool> Actualizar(Usuario usuario);
        Task<bool> Eliminar(int idUsuario);
    }
}
