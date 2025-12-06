using Microsoft.Data.SqlClient;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public class EspecialidadData : IEspecialidadData
    {
        private readonly string _connectionString;

        public EspecialidadData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PoliclinicoDb") 
                ?? throw new InvalidOperationException("Connection string 'PoliclinicoDb' not found.");
        }

        public async Task<List<Especialidad>> ListarTodas()
        {
            var especialidades = new List<Especialidad>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT IdEspecialidad, Nombre, Descripcion, Activo FROM Especialidades", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                especialidades.Add(new Especialidad
                {
                    IdEspecialidad = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Activo = reader.GetBoolean(3)
                });
            }

            return especialidades;
        }

        public async Task<List<Especialidad>> ListarActivas()
        {
            var especialidades = await ListarTodas();
            return especialidades.Where(e => e.Activo).ToList();
        }

        public async Task<Especialidad?> ObtenerPorId(int idEspecialidad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ObtenerEspecialidadPorId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdEspecialidad", idEspecialidad);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Especialidad
                {
                    IdEspecialidad = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Activo = reader.GetBoolean(3)
                };
            }

            return null;
        }

        public async Task<bool> Crear(Especialidad especialidad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CrearEspecialidad", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Nombre", especialidad.Nombre);
            command.Parameters.AddWithValue("@Descripcion", (object?)especialidad.Descripcion ?? DBNull.Value);

            await connection.OpenAsync();
            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Actualizar(Especialidad especialidad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ActualizarEspecialidad", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdEspecialidad", especialidad.IdEspecialidad);
            command.Parameters.AddWithValue("@Nombre", especialidad.Nombre);
            command.Parameters.AddWithValue("@Descripcion", (object?)especialidad.Descripcion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Activo", especialidad.Activo);

            await connection.OpenAsync();
            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Eliminar(int idEspecialidad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_EliminarEspecialidad", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdEspecialidad", idEspecialidad);

            await connection.OpenAsync();
            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
