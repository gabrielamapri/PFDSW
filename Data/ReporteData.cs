using System.Data;
using Microsoft.Data.SqlClient;

namespace PoliclinicoWeb.Data
{
    public class ReporteData : IReporteData
    {
        private readonly string _connectionString = null!;

        public ReporteData(IConfiguration configuration)
        {
            // Use the same connection string key as the rest of the app and fail fast if missing
            _connectionString = configuration.GetConnectionString("PoliclinicoDb") ?? throw new InvalidOperationException("Connection string 'PoliclinicoDb' is not configured.");
        }

        public List<ReporteCitasPorEspecialidad> ObtenerCitasPorEspecialidad(string fechaInicio, string fechaFin)
        {
            var lista = new List<ReporteCitasPorEspecialidad>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        e.Nombre as NombreEspecialidad,
                        COUNT(*) as TotalCitas,
                        SUM(CASE WHEN c.Estado = 'Pendiente' THEN 1 ELSE 0 END) as CitasPendientes,
                        SUM(CASE WHEN c.Estado = 'Confirmada' THEN 1 ELSE 0 END) as CitasConfirmadas,
                        SUM(CASE WHEN c.Estado = 'Atendida' THEN 1 ELSE 0 END) as CitasAtendidas,
                        SUM(CASE WHEN c.Estado = 'Cancelada' THEN 1 ELSE 0 END) as CitasCanceladas
                    FROM Citas c
                    INNER JOIN Doctores d ON c.IdDoctor = d.IdDoctor
                    INNER JOIN Especialidades e ON d.IdEspecialidad = e.IdEspecialidad
                    WHERE (@FechaInicio IS NULL OR c.FechaCita >= @FechaInicio)
                      AND (@FechaFin IS NULL OR c.FechaCita <= @FechaFin)
                    GROUP BY e.Nombre
                    ORDER BY TotalCitas DESC";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FechaInicio", string.IsNullOrEmpty(fechaInicio) ? (object)DBNull.Value : fechaInicio);
                command.Parameters.AddWithValue("@FechaFin", string.IsNullOrEmpty(fechaFin) ? (object)DBNull.Value : fechaFin);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ReporteCitasPorEspecialidad
                        {
                            NombreEspecialidad = reader.IsDBNull(reader.GetOrdinal("NombreEspecialidad")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreEspecialidad")),
                            TotalCitas = reader.IsDBNull(reader.GetOrdinal("TotalCitas")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalCitas")),
                            CitasPendientes = reader.IsDBNull(reader.GetOrdinal("CitasPendientes")) ? 0 : reader.GetInt32(reader.GetOrdinal("CitasPendientes")),
                            CitasConfirmadas = reader.IsDBNull(reader.GetOrdinal("CitasConfirmadas")) ? 0 : reader.GetInt32(reader.GetOrdinal("CitasConfirmadas")),
                            CitasAtendidas = reader.IsDBNull(reader.GetOrdinal("CitasAtendidas")) ? 0 : reader.GetInt32(reader.GetOrdinal("CitasAtendidas")),
                            CitasCanceladas = reader.IsDBNull(reader.GetOrdinal("CitasCanceladas")) ? 0 : reader.GetInt32(reader.GetOrdinal("CitasCanceladas"))
                        });
                    }
                }
            }

            return lista;
        }

        public List<ReporteCitasPorDoctor> ObtenerCitasPorDoctor(string fechaInicio, string fechaFin, string estado)
        {
            var lista = new List<ReporteCitasPorDoctor>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        d.Nombres + ' ' + d.Apellidos as NombreDoctor,
                        e.Nombre as Especialidad,
                        COUNT(*) as TotalCitas,
                        SUM(CASE WHEN c.Estado = 'Pendiente' THEN 1 ELSE 0 END) as CitasPendientes,
                        SUM(CASE WHEN c.Estado = 'Confirmada' THEN 1 ELSE 0 END) as CitasConfirmadas,
                        SUM(CASE WHEN c.Estado = 'Atendida' THEN 1 ELSE 0 END) as CitasAtendidas,
                        SUM(CASE WHEN c.Estado = 'Cancelada' THEN 1 ELSE 0 END) as CitasCanceladas
                    FROM Citas c
                    INNER JOIN Doctores d ON c.IdDoctor = d.IdDoctor
                    INNER JOIN Especialidades e ON d.IdEspecialidad = e.IdEspecialidad
                    WHERE (@FechaInicio IS NULL OR c.FechaCita >= @FechaInicio)
                      AND (@FechaFin IS NULL OR c.FechaCita <= @FechaFin)
                      AND (@Estado IS NULL OR c.Estado = @Estado)
                    GROUP BY d.Nombres, d.Apellidos, e.Nombre
                    ORDER BY TotalCitas DESC";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FechaInicio", string.IsNullOrEmpty(fechaInicio) ? (object)DBNull.Value : fechaInicio);
                command.Parameters.AddWithValue("@FechaFin", string.IsNullOrEmpty(fechaFin) ? (object)DBNull.Value : fechaFin);
                command.Parameters.AddWithValue("@Estado", string.IsNullOrEmpty(estado) ? (object)DBNull.Value : estado);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ReporteCitasPorDoctor
                        {
                            NombreDoctor = reader.IsDBNull(reader.GetOrdinal("NombreDoctor")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreDoctor")),
                            Especialidad = reader.IsDBNull(reader.GetOrdinal("Especialidad")) ? string.Empty : reader.GetString(reader.GetOrdinal("Especialidad")),
                            TotalCitas = reader.IsDBNull(reader.GetOrdinal("TotalCitas")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalCitas")),
                            CitasPendientes = reader.IsDBNull(reader.GetOrdinal("CitasPendientes")) ? 0 : reader.GetInt32(reader.GetOrdinal("CitasPendientes")),
                            CitasConfirmadas = reader.IsDBNull(reader.GetOrdinal("CitasConfirmadas")) ? 0 : reader.GetInt32(reader.GetOrdinal("CitasConfirmadas")),
                            CitasAtendidas = reader.IsDBNull(reader.GetOrdinal("CitasAtendidas")) ? 0 : reader.GetInt32(reader.GetOrdinal("CitasAtendidas")),
                            CitasCanceladas = reader.IsDBNull(reader.GetOrdinal("CitasCanceladas")) ? 0 : reader.GetInt32(reader.GetOrdinal("CitasCanceladas"))
                        });
                    }
                }
            }

            return lista;
        }

        public List<ReporteCitasPorEstado> ObtenerCitasPorEstado()
        {
            var lista = new List<ReporteCitasPorEstado>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        Estado,
                        COUNT(*) as Total
                    FROM Citas
                    GROUP BY Estado
                    ORDER BY Total DESC";

                var command = new SqlCommand(query, connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ReporteCitasPorEstado
                        {
                            Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? string.Empty : reader.GetString(reader.GetOrdinal("Estado")),
                            Total = reader.IsDBNull(reader.GetOrdinal("Total")) ? 0 : reader.GetInt32(reader.GetOrdinal("Total"))
                        });
                    }
                }
            }

            return lista;
        }

        public List<ReportePacientesFrecuentes> ObtenerPacientesFrecuentes(int top)
        {
            var lista = new List<ReportePacientesFrecuentes>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT TOP (@Top)
                        p.Nombres + ' ' + p.Apellidos as NombrePaciente,
                        p.DNI,
                        COUNT(*) as TotalCitas,
                        MAX(c.FechaCita) as UltimaCita
                    FROM Citas c
                    INNER JOIN Pacientes p ON c.IdPaciente = p.IdPaciente
                    GROUP BY p.Nombres, p.Apellidos, p.DNI
                    ORDER BY TotalCitas DESC";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Top", top);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ReportePacientesFrecuentes
                        {
                            NombrePaciente = reader.IsDBNull(reader.GetOrdinal("NombrePaciente")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombrePaciente")),
                            DNI = reader.IsDBNull(reader.GetOrdinal("DNI")) ? string.Empty : reader.GetString(reader.GetOrdinal("DNI")),
                            TotalCitas = reader.IsDBNull(reader.GetOrdinal("TotalCitas")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalCitas")),
                            UltimaCita = reader.IsDBNull(reader.GetOrdinal("UltimaCita")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UltimaCita"))
                        });
                    }
                }
            }

            return lista;
        }
    }
}
