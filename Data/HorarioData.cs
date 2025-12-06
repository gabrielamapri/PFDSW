using Microsoft.Data.SqlClient;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public class HorarioData : IHorarioData
    {
        private readonly string _connectionString;

        public HorarioData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PoliclinicoDb") 
                ?? throw new InvalidOperationException("Connection string 'PoliclinicoDb' not found.");
        }

        public async Task<List<Horario>> ListarPorDoctor(int idDoctor)
        {
            var horarios = new List<Horario>();
            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT h.*, d.Nombres + ' ' + d.Apellidos as NombreDoctor 
                         FROM Horarios h 
                         LEFT JOIN Doctores d ON h.IdDoctor = d.IdDoctor 
                         WHERE h.IdDoctor = @IdDoctor 
                         ORDER BY 
                             CASE h.DiaSemana 
                                 WHEN 'Lunes' THEN 1 
                                 WHEN 'Martes' THEN 2 
                                 WHEN 'Miércoles' THEN 3 
                                 WHEN 'Jueves' THEN 4 
                                 WHEN 'Viernes' THEN 5 
                                 WHEN 'Sábado' THEN 6 
                                 WHEN 'Domingo' THEN 7 
                             END, h.HoraInicio";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdDoctor", idDoctor);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) { horarios.Add(MapearHorario(reader)); }
            return horarios;
        }

        public async Task<Horario?> ObtenerPorId(int idHorario)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT h.*, d.Nombres + ' ' + d.Apellidos as NombreDoctor 
                         FROM Horarios h 
                         LEFT JOIN Doctores d ON h.IdDoctor = d.IdDoctor 
                         WHERE h.IdHorario = @IdHorario";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdHorario", idHorario);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) { return MapearHorario(reader); }
            return null;
        }

        public async Task<bool> Crear(Horario horario)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"INSERT INTO Horarios (IdDoctor, DiaSemana, HoraInicio, HoraFin, DuracionCitaMinutos, Activo) 
                         VALUES (@IdDoctor, @DiaSemana, @HoraInicio, @HoraFin, @DuracionCitaMinutos, @Activo)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdDoctor", horario.IdDoctor);
            command.Parameters.AddWithValue("@DiaSemana", horario.DiaSemana ?? string.Empty);
            command.Parameters.AddWithValue("@HoraInicio", horario.HoraInicio);
            command.Parameters.AddWithValue("@HoraFin", horario.HoraFin);
            command.Parameters.AddWithValue("@DuracionCitaMinutos", horario.DuracionCitaMinutos);
            command.Parameters.AddWithValue("@Activo", horario.Activo);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> Actualizar(Horario horario)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"UPDATE Horarios 
                         SET DiaSemana = @DiaSemana, 
                             HoraInicio = @HoraInicio, 
                             HoraFin = @HoraFin, 
                             DuracionCitaMinutos = @DuracionCitaMinutos, 
                             Activo = @Activo 
                         WHERE IdHorario = @IdHorario";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdHorario", horario.IdHorario);
            command.Parameters.AddWithValue("@DiaSemana", horario.DiaSemana);
            command.Parameters.AddWithValue("@HoraInicio", horario.HoraInicio);
            command.Parameters.AddWithValue("@HoraFin", horario.HoraFin);
            command.Parameters.AddWithValue("@DuracionCitaMinutos", horario.DuracionCitaMinutos);
            command.Parameters.AddWithValue("@Activo", horario.Activo);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> Eliminar(int idHorario)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = "DELETE FROM Horarios WHERE IdHorario = @IdHorario";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdHorario", idHorario);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<TimeSpan>> ObtenerHorasDisponibles(int idDoctor, DateTime fecha)
        {
            var horas = new List<TimeSpan>();
            var diaSemana = ObtenerNombreDia(fecha.DayOfWeek);

            using var connection = new SqlConnection(_connectionString);
            
            // 1. Obtener horario del doctor para ese día
            var queryHorario = @"SELECT HoraInicio, HoraFin, DuracionCitaMinutos 
                                FROM Horarios 
                                WHERE IdDoctor = @IdDoctor 
                                  AND DiaSemana = @DiaSemana 
                                  AND Activo = 1";
            
            using var commandHorario = new SqlCommand(queryHorario, connection);
            commandHorario.Parameters.AddWithValue("@IdDoctor", idDoctor);
            commandHorario.Parameters.AddWithValue("@DiaSemana", diaSemana);
            
            await connection.OpenAsync();
            using var readerHorario = await commandHorario.ExecuteReaderAsync();
            
            if (!await readerHorario.ReadAsync())
            {
                return horas; // No tiene horario ese día
            }

            var horaInicio = readerHorario.GetTimeSpan(0);
            var horaFin = readerHorario.GetTimeSpan(1);
            var duracion = readerHorario.GetInt32(2);
            await readerHorario.CloseAsync();

            // 2. Obtener citas ocupadas
            var queryCitas = @"SELECT HoraCita 
                              FROM Citas 
                              WHERE IdDoctor = @IdDoctor 
                                AND CAST(FechaCita AS DATE) = @Fecha 
                                AND Estado NOT IN ('Cancelada')";
            
            var horasOcupadas = new List<TimeSpan>();
            using var commandCitas = new SqlCommand(queryCitas, connection);
            commandCitas.Parameters.AddWithValue("@IdDoctor", idDoctor);
            commandCitas.Parameters.AddWithValue("@Fecha", fecha.Date);
            
            using var readerCitas = await commandCitas.ExecuteReaderAsync();
            while (await readerCitas.ReadAsync())
            {
                horasOcupadas.Add(readerCitas.GetTimeSpan(0));
            }

            // 3. Generar slots disponibles
            var horaActual = horaInicio;
            while (horaActual < horaFin)
            {
                if (!horasOcupadas.Contains(horaActual))
                {
                    horas.Add(horaActual);
                }
                horaActual = horaActual.Add(TimeSpan.FromMinutes(duracion));
            }

            return horas;
        }

        private string ObtenerNombreDia(DayOfWeek dia)
        {
            return dia switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => ""
            };
        }

        private Horario MapearHorario(SqlDataReader reader)
        {
            return new Horario
            {
                IdHorario = reader.GetInt32(reader.GetOrdinal("IdHorario")),
                IdDoctor = reader.GetInt32(reader.GetOrdinal("IdDoctor")),
                NombreDoctor = reader.IsDBNull(reader.GetOrdinal("NombreDoctor")) ? null : reader.GetString(reader.GetOrdinal("NombreDoctor")),
                DiaSemana = reader.GetString(reader.GetOrdinal("DiaSemana")),
                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                DuracionCitaMinutos = reader.GetInt32(reader.GetOrdinal("DuracionCitaMinutos")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo"))
            };
        }
    }
}
