using Microsoft.Data.SqlClient;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public class CitaData : ICitaData
    {
        private readonly string _connectionString;

        public CitaData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PoliclinicoDb") 
                ?? throw new InvalidOperationException("Connection string 'PoliclinicoDb' not found.");
        }

        public async Task<List<Cita>> ListarTodas()
        {
            var citas = new List<Cita>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ListarCitas", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) { citas.Add(MapearCita(reader)); }
            return citas;
        }

        public async Task<List<Cita>> ListarPorPaciente(int idPaciente)
        {
            var citas = new List<Cita>();
            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT c.*, 
                                 p.Nombres + ' ' + p.Apellidos as NombrePaciente,
                                 d.Nombres + ' ' + d.Apellidos as NombreDoctor,
                                 e.Nombre as NombreEspecialidad
                         FROM Citas c
                         LEFT JOIN Pacientes p ON c.IdPaciente = p.IdPaciente
                         LEFT JOIN Doctores d ON c.IdDoctor = d.IdDoctor
                         LEFT JOIN Especialidades e ON d.IdEspecialidad = e.IdEspecialidad
                         WHERE c.IdPaciente = @IdPaciente
                         ORDER BY c.FechaCita DESC, c.HoraCita DESC";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdPaciente", idPaciente);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) { citas.Add(MapearCita(reader)); }
            return citas;
        }

        public async Task<List<Cita>> ListarPorDoctor(int idDoctor)
        {
            var citas = new List<Cita>();
            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT c.*, 
                                 p.Nombres + ' ' + p.Apellidos as NombrePaciente,
                                 d.Nombres + ' ' + d.Apellidos as NombreDoctor,
                                 e.Nombre as NombreEspecialidad
                         FROM Citas c
                         LEFT JOIN Pacientes p ON c.IdPaciente = p.IdPaciente
                         LEFT JOIN Doctores d ON c.IdDoctor = d.IdDoctor
                         LEFT JOIN Especialidades e ON d.IdEspecialidad = e.IdEspecialidad
                         WHERE c.IdDoctor = @IdDoctor
                         ORDER BY c.FechaCita ASC, c.HoraCita ASC";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdDoctor", idDoctor);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) { citas.Add(MapearCita(reader)); }
            return citas;
        }

        public async Task<List<Cita>> ListarPorFecha(DateTime fecha)
        {
            var citas = new List<Cita>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ListarCitasPorFecha", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@Fecha", fecha);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) { citas.Add(MapearCita(reader)); }
            return citas;
        }

        public async Task<PaginacionViewModel<Cita>> ListarConFiltros(int pagina, int registrosPorPagina, 
            int? idEspecialidad = null, int? idDoctor = null, DateTime? fechaInicio = null, DateTime? fechaFin = null, string? estado = null)
        {
            var resultado = new PaginacionViewModel<Cita> { PaginaActual = pagina, RegistrosPorPagina = registrosPorPagina };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ListarCitasConFiltros", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@Pagina", pagina);
            command.Parameters.AddWithValue("@RegistrosPorPagina", registrosPorPagina);
            command.Parameters.AddWithValue("@IdEspecialidad", (object?)idEspecialidad ?? DBNull.Value);
            command.Parameters.AddWithValue("@IdDoctor", (object?)idDoctor ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaInicio", (object?)fechaInicio ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaFin", (object?)fechaFin ?? DBNull.Value);
            command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) { resultado.Items.Add(MapearCita(reader)); }
            if (await reader.NextResultAsync() && await reader.ReadAsync()) { resultado.TotalRegistros = reader.GetInt32(0); }
            resultado.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / registrosPorPagina);
            return resultado;
        }

        public async Task<Cita?> ObtenerPorId(int idCita)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ObtenerCitaPorId", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdCita", idCita);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) { return MapearCita(reader); }
            return null;
        }

        public async Task<bool> Crear(Cita cita)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"INSERT INTO Citas (IdPaciente, IdDoctor, FechaCita, HoraCita, Motivo, Estado, FechaRegistro) 
                         VALUES (@IdPaciente, @IdDoctor, @FechaCita, @HoraCita, @Motivo, @Estado, @FechaRegistro)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdPaciente", cita.IdPaciente);
            command.Parameters.AddWithValue("@IdDoctor", cita.IdDoctor);
            command.Parameters.AddWithValue("@FechaCita", cita.FechaCita);
            command.Parameters.AddWithValue("@HoraCita", cita.HoraCita);
            command.Parameters.AddWithValue("@Motivo", string.IsNullOrEmpty(cita.Motivo) ? DBNull.Value : cita.Motivo);
            command.Parameters.AddWithValue("@Estado", cita.Estado);
            command.Parameters.AddWithValue("@FechaRegistro", cita.FechaRegistro);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> Actualizar(Cita cita)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ActualizarCita", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdCita", cita.IdCita);
            command.Parameters.AddWithValue("@FechaCita", cita.FechaCita);
            command.Parameters.AddWithValue("@HoraCita", cita.HoraCita);
            command.Parameters.AddWithValue("@Estado", cita.Estado);
            command.Parameters.AddWithValue("@Observaciones", (object?)cita.Observaciones ?? DBNull.Value);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> CambiarEstado(int idCita, string nuevoEstado)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = "UPDATE Citas SET Estado = @NuevoEstado WHERE IdCita = @IdCita";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdCita", idCita);
            command.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> Eliminar(int idCita)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_EliminarCita", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdCita", idCita);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> VerificarDisponibilidad(int idDoctor, DateTime fecha, TimeSpan hora)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_VerificarDisponibilidadCita", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdDoctor", idDoctor);
            command.Parameters.AddWithValue("@Fecha", fecha);
            command.Parameters.AddWithValue("@Hora", hora);
            var outputParam = new SqlParameter("@Disponible", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
            command.Parameters.Add(outputParam);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return (bool)outputParam.Value;
        }

        private Cita MapearCita(SqlDataReader reader)
        {
            return new Cita
            {
                IdCita = reader.GetInt32(reader.GetOrdinal("IdCita")),
                IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                NombrePaciente = reader.IsDBNull(reader.GetOrdinal("NombrePaciente")) ? null : reader.GetString(reader.GetOrdinal("NombrePaciente")),
                IdDoctor = reader.GetInt32(reader.GetOrdinal("IdDoctor")),
                NombreDoctor = reader.IsDBNull(reader.GetOrdinal("NombreDoctor")) ? null : reader.GetString(reader.GetOrdinal("NombreDoctor")),
                NombreEspecialidad = reader.IsDBNull(reader.GetOrdinal("NombreEspecialidad")) ? null : reader.GetString(reader.GetOrdinal("NombreEspecialidad")),
                FechaCita = reader.GetDateTime(reader.GetOrdinal("FechaCita")),
                HoraCita = reader.GetTimeSpan(reader.GetOrdinal("HoraCita")),
                Estado = reader.GetString(reader.GetOrdinal("Estado")),
                Motivo = reader.IsDBNull(reader.GetOrdinal("Motivo")) ? null : reader.GetString(reader.GetOrdinal("Motivo")),
                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                MontoPagado = reader.IsDBNull(reader.GetOrdinal("MontoPagado")) ? null : reader.GetDecimal(reader.GetOrdinal("MontoPagado")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
            };
        }
    }
}
