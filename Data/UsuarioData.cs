using Microsoft.Data.SqlClient;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public class UsuarioData : IUsuarioData
    {
        private readonly string _connectionString;

        public UsuarioData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PoliclinicoDb") 
                ?? throw new InvalidOperationException("Connection string 'PoliclinicoDb' not found.");
        }

        public async Task<Usuario?> ValidarUsuario(string nombreUsuario, string contraseña)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ValidarUsuario", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
            command.Parameters.AddWithValue("@Contraseña", contraseña);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32(0),
                    NombreUsuario = reader.GetString(1),
                    TipoUsuario = reader.GetString(2),
                    IdRelacionado = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Activo = reader.GetBoolean(4)
                };
            }

            return null;
        }

        public async Task<Usuario?> ObtenerPorId(int idUsuario)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ObtenerUsuarioPorId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32(0),
                    NombreUsuario = reader.GetString(1),
                    Contraseña = reader.GetString(2),
                    TipoUsuario = reader.GetString(3),
                    IdRelacionado = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    Activo = reader.GetBoolean(5),
                    FechaCreacion = reader.GetDateTime(6)
                };
            }

            return null;
        }

        public async Task<List<Usuario>> ListarTodos()
        {
            var usuarios = new List<Usuario>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ListarUsuarios", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new Usuario
                {
                    IdUsuario = reader.GetInt32(0),
                    NombreUsuario = reader.GetString(1),
                    TipoUsuario = reader.GetString(2),
                    IdRelacionado = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Activo = reader.GetBoolean(4),
                    FechaCreacion = reader.GetDateTime(5)
                });
            }

            return usuarios;
        }

        public async Task<bool> Crear(Usuario usuario)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CrearUsuario", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
            command.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
            command.Parameters.AddWithValue("@TipoUsuario", usuario.TipoUsuario);
            command.Parameters.AddWithValue("@IdRelacionado", (object?)usuario.IdRelacionado ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteNonQueryAsync();

            return result > 0;
        }

        public async Task<bool> Actualizar(Usuario usuario)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ActualizarUsuario", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
            command.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
            command.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
            command.Parameters.AddWithValue("@TipoUsuario", usuario.TipoUsuario);
            command.Parameters.AddWithValue("@Activo", usuario.Activo);

            await connection.OpenAsync();
            var result = await command.ExecuteNonQueryAsync();

            return result > 0;
        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_EliminarUsuario", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            await connection.OpenAsync();
            var result = await command.ExecuteNonQueryAsync();

            return result > 0;
        }
    }
}
