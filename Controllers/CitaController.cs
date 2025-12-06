using Microsoft.AspNetCore.Mvc;
using PoliclinicoWeb.Data;
using PoliclinicoWeb.Models;
// SignalR removed: no hub using required

namespace PoliclinicoWeb.Controllers
{
    public class CitaController : Controller
    {
        private readonly ICitaData _citaData;
        private readonly IEspecialidadData _especialidadData;
        private readonly IDoctorData _doctorData;
        private readonly IHorarioData _horarioData;

        public CitaController(ICitaData citaData, IEspecialidadData especialidadData, IDoctorData doctorData, IHorarioData horarioData)
        {
            _citaData = citaData;
            _especialidadData = especialidadData;
            _doctorData = doctorData;
            _horarioData = horarioData;
        }

        // Vista principal para reservar cita (solo pacientes)
        public IActionResult ReservarCita()
        {
            if (HttpContext.Session.GetString("TipoUsuario") != "Paciente")
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }
            return View();
        }

        // Ver mis citas (paciente)
        public async Task<IActionResult> MisCitas()
        {
            if (HttpContext.Session.GetString("TipoUsuario") != "Paciente")
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            var idPaciente = HttpContext.Session.GetInt32("IdPaciente");
            if (idPaciente == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var citas = await _citaData.ListarPorPaciente(idPaciente.Value);
            return View(citas);
        }

        // Ver agenda del doctor
        public async Task<IActionResult> MiAgenda()
        {
            if (HttpContext.Session.GetString("TipoUsuario") != "Doctor")
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            var idDoctor = HttpContext.Session.GetInt32("IdDoctor");
            if (idDoctor == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var citas = await _citaData.ListarPorDoctor(idDoctor.Value);
            return View(citas);
        }

        // API: Obtener especialidades para el primer paso
        [HttpGet]
        public async Task<IActionResult> ObtenerEspecialidades()
        {
            var especialidades = await _especialidadData.ListarActivas();
            return Json(especialidades);
        }

        // API: Obtener doctores por especialidad
        [HttpGet]
        public async Task<IActionResult> ObtenerDoctoresPorEspecialidad(int idEspecialidad)
        {
            var doctores = await _doctorData.ListarPorEspecialidad(idEspecialidad);
            return Json(doctores.Where(d => d.Activo).ToList());
        }

        // API: Obtener días de atención de un doctor
        [HttpGet]
        public async Task<IActionResult> ObtenerDiasAtencion(int idDoctor)
        {
            var horarios = await _horarioData.ListarPorDoctor(idDoctor);
            var diasActivos = horarios
                .Where(h => h.Activo)
                .Select(h => h.DiaSemana)
                .Distinct()
                .ToList();
            return Json(diasActivos);
        }

        // API: Obtener horarios disponibles de un doctor
        [HttpGet]
        public async Task<IActionResult> ObtenerHorariosDisponibles(int idDoctor, string fecha)
        {
            if (!DateTime.TryParse(fecha, out var fechaCita))
            {
                return BadRequest("Fecha inválida");
            }

            var horarios = await _horarioData.ObtenerHorasDisponibles(idDoctor, fechaCita);
            return Json(horarios);
        }

        // API: Crear cita
        [HttpPost]
        public async Task<IActionResult> CrearCita([FromBody] CitaDto dto)
        {
            try
            {
                var idPaciente = HttpContext.Session.GetInt32("IdPaciente");
                if (idPaciente == null)
                {
                    return Json(new { success = false, message = "Sesión expirada" });
                }

                var cita = new Cita
                {
                    IdPaciente = idPaciente.Value,
                    IdDoctor = dto.IdDoctor,
                    FechaCita = DateTime.Parse(dto.FechaCita),
                    HoraCita = TimeSpan.Parse(dto.HoraCita),
                    Motivo = dto.Motivo,
                    Estado = "Pendiente",
                    FechaRegistro = DateTime.Now
                };

                var resultado = await _citaData.Crear(cita);
                if (resultado)
                {
                    // SignalR removed: no realtime notification here
                    return Json(new { success = true, message = "Cita reservada exitosamente" });
                }
                return Json(new { success = false, message = "Error al reservar la cita" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Cancelar cita
        [HttpPost]
        public async Task<IActionResult> CancelarCita(int idCita)
        {
            try
            {
                var resultado = await _citaData.CambiarEstado(idCita, "Cancelada");
                return Json(new { success = resultado, message = resultado ? "Cita cancelada" : "Error al cancelar" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Confirmar cita (doctor)
        [HttpPost]
        public async Task<IActionResult> ConfirmarCita(int idCita)
        {
            try
            {
                var resultado = await _citaData.CambiarEstado(idCita, "Confirmada");
                return Json(new { success = resultado, message = resultado ? "Cita confirmada" : "Error" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Marcar como atendida (doctor)
        [HttpPost]
        public async Task<IActionResult> MarcarAtendida(int idCita)
        {
            try
            {
                var resultado = await _citaData.CambiarEstado(idCita, "Atendida");
                return Json(new { success = resultado, message = resultado ? "Cita marcada como atendida" : "Error" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO para recibir datos desde JavaScript
    public class CitaDto
    {
        public int IdDoctor { get; set; }
        public string FechaCita { get; set; } = string.Empty; // "2025-12-10"
        public string HoraCita { get; set; } = string.Empty;  // "08:00"
        public string? Motivo { get; set; }
    }
}
