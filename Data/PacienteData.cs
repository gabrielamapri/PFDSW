using Microsoft.Data.SqlClient;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public class PacienteData : IPacienteData
    {
        private readonly string _connectionString;

        public PacienteData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PoliclinicoDb") 
                ?? throw new InvalidOperationException("Connection string 'PoliclinicoDb' not found.");
        }

        public async Task<List<Paciente>> ListarTodos()
        {
            var pacientes = new List<Paciente>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT IdPaciente, Nombres, Apellidos, DNI, FechaNacimiento, Sexo, Direccion, Telefono, Email, GrupoSanguineo, Activo, FechaRegistro FROM Pacientes", connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) { pacientes.Add(MapearPaciente(reader)); }
            return pacientes;
        }

        public async Task<PaginacionViewModel<Paciente>> ListarConPaginacion(int pagina, int registrosPorPagina, string? busqueda = null)
        {
            var resultado = new PaginacionViewModel<Paciente> { PaginaActual = pagina, RegistrosPorPagina = registrosPorPagina };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ListarPacientesPaginado", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@Pagina", pagina);
            command.Parameters.AddWithValue("@RegistrosPorPagina", registrosPorPagina);
            command.Parameters.AddWithValue("@Busqueda", (object?)busqueda ?? DBNull.Value);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) { resultado.Items.Add(MapearPaciente(reader)); }
            if (await reader.NextResultAsync() && await reader.ReadAsync()) { resultado.TotalRegistros = reader.GetInt32(0); }
            resultado.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / registrosPorPagina);
            return resultado;
        }

        public async Task<Paciente?> ObtenerPorId(int idPaciente)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ObtenerPacientePorId", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdPaciente", idPaciente);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) { return MapearPaciente(reader); }
            return null;
        }

        public async Task<Paciente?> ObtenerPorDNI(string dni)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ObtenerPacientePorDNI", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@DNI", dni);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) { return MapearPaciente(reader); }
            return null;
        }

        public async Task<bool> Crear(Paciente paciente)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CrearPaciente", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@Nombres", paciente.Nombres);
            command.Parameters.AddWithValue("@Apellidos", paciente.Apellidos);
            command.Parameters.AddWithValue("@DNI", paciente.DNI);
            command.Parameters.AddWithValue("@FechaNacimiento", paciente.FechaNacimiento);
            command.Parameters.AddWithValue("@Sexo", (object?)paciente.Sexo ?? DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", (object?)paciente.Direccion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Telefono", (object?)paciente.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)paciente.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@GrupoSanguineo", (object?)paciente.GrupoSanguineo ?? DBNull.Value);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> Actualizar(Paciente paciente)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ActualizarPaciente", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdPaciente", paciente.IdPaciente);
            command.Parameters.AddWithValue("@Nombres", paciente.Nombres);
            command.Parameters.AddWithValue("@Apellidos", paciente.Apellidos);
            command.Parameters.AddWithValue("@DNI", paciente.DNI);
            command.Parameters.AddWithValue("@FechaNacimiento", paciente.FechaNacimiento);
            command.Parameters.AddWithValue("@Sexo", (object?)paciente.Sexo ?? DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", (object?)paciente.Direccion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Telefono", (object?)paciente.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)paciente.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@GrupoSanguineo", (object?)paciente.GrupoSanguineo ?? DBNull.Value);
            command.Parameters.AddWithValue("@Activo", paciente.Activo);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> Eliminar(int idPaciente)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_EliminarPaciente", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdPaciente", idPaciente);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        private Paciente MapearPaciente(SqlDataReader reader)
        {
            return new Paciente
            {
                IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                Apellidos = reader.GetString(reader.GetOrdinal("Apellidos")),
                DNI = reader.GetString(reader.GetOrdinal("DNI")),
                FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                Sexo = reader.IsDBNull(reader.GetOrdinal("Sexo")) ? null : reader.GetString(reader.GetOrdinal("Sexo")),
                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                GrupoSanguineo = reader.IsDBNull(reader.GetOrdinal("GrupoSanguineo")) ? null : reader.GetString(reader.GetOrdinal("GrupoSanguineo")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
            };
        }
    }
}
