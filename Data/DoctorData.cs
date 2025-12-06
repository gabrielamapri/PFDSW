using Microsoft.Data.SqlClient;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public class DoctorData : IDoctorData
    {
        private readonly string _connectionString;

        public DoctorData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PoliclinicoDb") 
                ?? throw new InvalidOperationException("Connection string 'PoliclinicoDb' not found.");
        }

        public async Task<List<Doctor>> ListarTodos()
        {
            var doctores = new List<Doctor>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(@"
                  SELECT d.IdDoctor, d.Nombres, d.Apellidos, d.DNI, d.IdEspecialidad, 
                      e.Nombre as NombreEspecialidad, d.Telefono, d.Email, d.CMP, d.RNE, d.Direccion, d.Activo, d.FechaRegistro
                FROM Doctores d
                LEFT JOIN Especialidades e ON d.IdEspecialidad = e.IdEspecialidad", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                doctores.Add(MapearDoctor(reader));
            }

            return doctores;
        }

        public async Task<List<Doctor>> ListarActivos()
        {
            var doctores = await ListarTodos();
            return doctores.Where(d => d.Activo).ToList();
        }

        public async Task<List<Doctor>> ListarPorEspecialidad(int idEspecialidad)
        {
            var doctores = new List<Doctor>();

            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT d.*, e.Nombre as NombreEspecialidad 
                         FROM Doctores d 
                         LEFT JOIN Especialidades e ON d.IdEspecialidad = e.IdEspecialidad 
                         WHERE d.IdEspecialidad = @IdEspecialidad AND d.Activo = 1 
                         ORDER BY d.Nombres, d.Apellidos";
            
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdEspecialidad", idEspecialidad);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                doctores.Add(MapearDoctor(reader));
            }

            return doctores;
        }

        public async Task<Doctor?> ObtenerPorId(int idDoctor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ObtenerDoctorPorId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdDoctor", idDoctor);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapearDoctor(reader);
            }

            return null;
        }

        public async Task<bool> Crear(Doctor doctor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CrearDoctor", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Nombres", doctor.Nombres);
            command.Parameters.AddWithValue("@Apellidos", doctor.Apellidos);
            command.Parameters.AddWithValue("@DNI", doctor.DNI);
            command.Parameters.AddWithValue("@IdEspecialidad", doctor.IdEspecialidad);
            command.Parameters.AddWithValue("@Telefono", (object?)doctor.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)doctor.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@CMP", (object?)doctor.CMP ?? DBNull.Value);
            command.Parameters.AddWithValue("@RNE", (object?)doctor.RNE ?? DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", (object?)doctor.Direccion ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            // Stored procedures use SET NOCOUNT ON, so ExecuteNonQuery may return -1.
            // If no exception was thrown, consider the operation successful.
            return true;
        }

        public async Task<bool> Actualizar(Doctor doctor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ActualizarDoctor", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdDoctor", doctor.IdDoctor);
            command.Parameters.AddWithValue("@Nombres", doctor.Nombres);
            command.Parameters.AddWithValue("@Apellidos", doctor.Apellidos);
            command.Parameters.AddWithValue("@DNI", doctor.DNI);
            command.Parameters.AddWithValue("@IdEspecialidad", doctor.IdEspecialidad);
            command.Parameters.AddWithValue("@Telefono", (object?)doctor.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)doctor.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@CMP", (object?)doctor.CMP ?? DBNull.Value);
            command.Parameters.AddWithValue("@RNE", (object?)doctor.RNE ?? DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", (object?)doctor.Direccion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Activo", doctor.Activo);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> Eliminar(int idDoctor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_EliminarDoctor", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdDoctor", idDoctor);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        private Doctor MapearDoctor(SqlDataReader reader)
        {
            return new Doctor
            {
                IdDoctor = reader.GetInt32(reader.GetOrdinal("IdDoctor")),
                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                Apellidos = reader.GetString(reader.GetOrdinal("Apellidos")),
                DNI = reader.GetString(reader.GetOrdinal("DNI")),
                IdEspecialidad = reader.GetInt32(reader.GetOrdinal("IdEspecialidad")),
                NombreEspecialidad = reader.IsDBNull(reader.GetOrdinal("NombreEspecialidad")) ? null : reader.GetString(reader.GetOrdinal("NombreEspecialidad")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                CMP = reader.IsDBNull(reader.GetOrdinal("CMP")) ? null : reader.GetString(reader.GetOrdinal("CMP")),
                RNE = reader.IsDBNull(reader.GetOrdinal("RNE")) ? null : reader.GetString(reader.GetOrdinal("RNE")),
                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
            };
        }
    }
}
