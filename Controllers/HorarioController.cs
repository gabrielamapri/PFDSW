using Microsoft.AspNetCore.Mvc;
using PoliclinicoWeb.Data;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Controllers
{
    public class HorarioController : Controller
    {
        private readonly IHorarioData _horarioData;
        private readonly IDoctorData _doctorData;

        public HorarioController(IHorarioData horarioData, IDoctorData doctorData)
        {
            _horarioData = horarioData;
            _doctorData = doctorData;
        }

        // GET: /Horario/Index (Admin ve todos, Doctor ve solo los suyos)
        public IActionResult Index()
        {
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");
            if (tipoUsuario != "Admin" && tipoUsuario != "Doctor")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.TipoUsuario = tipoUsuario;
            ViewBag.IdDoctor = HttpContext.Session.GetInt32("IdDoctor");
            return View();
        }

        // GET: /Horario/ObtenerDoctores (AJAX)
        [HttpGet]
        public async Task<IActionResult> ObtenerDoctores()
        {
            var doctores = await _doctorData.ListarTodos();
            return Json(doctores);
        }

        // GET: /Horario/ListarPorDoctor?idDoctor=1 (AJAX)
        [HttpGet]
        public async Task<IActionResult> ListarPorDoctor(int idDoctor)
        {
            var horarios = await _horarioData.ListarPorDoctor(idDoctor);
            return Json(new { data = horarios });
        }

        // POST: /Horario/Crear
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] HorarioDto dto)
        {
            try
            {
                var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");
                
                // Crear objeto Horario desde DTO
                var horario = new Horario
                {
                    IdDoctor = dto.IdDoctor,
                    DiaSemana = dto.DiaSemana,
                    HoraInicio = TimeSpan.Parse(dto.HoraInicio),
                    HoraFin = TimeSpan.Parse(dto.HoraFin),
                    DuracionCitaMinutos = dto.DuracionCitaMinutos,
                    Activo = true
                };
                
                // Si es doctor, asignar su propio ID
                if (tipoUsuario == "Doctor")
                {
                    horario.IdDoctor = HttpContext.Session.GetInt32("IdDoctor")!.Value;
                }
                
                // Validar que se haya seleccionado un doctor
                if (horario.IdDoctor <= 0)
                {
                    return Json(new { success = false, message = "Debe seleccionar un doctor" });
                }

                // Validar que HoraFin > HoraInicio
                if (horario.HoraFin <= horario.HoraInicio)
                {
                    return Json(new { success = false, message = "La hora de fin debe ser mayor a la hora de inicio" });
                }

                var resultado = await _horarioData.Crear(horario);
                
                if (resultado)
                {
                    return Json(new { success = true, message = "Horario creado exitosamente" });
                }
                return Json(new { success = false, message = "No se pudo crear el horario" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Horario/Actualizar
        [HttpPost]
        public async Task<IActionResult> Actualizar([FromBody] HorarioDto dto)
        {
            try
            {
                var horario = new Horario
                {
                    IdHorario = dto.IdHorario,
                    IdDoctor = dto.IdDoctor,
                    DiaSemana = dto.DiaSemana,
                    HoraInicio = TimeSpan.Parse(dto.HoraInicio),
                    HoraFin = TimeSpan.Parse(dto.HoraFin),
                    DuracionCitaMinutos = dto.DuracionCitaMinutos,
                    Activo = dto.Activo
                };
                
                if (horario.HoraFin <= horario.HoraInicio)
                {
                    return Json(new { success = false, message = "La hora de fin debe ser mayor a la hora de inicio" });
                }

                var resultado = await _horarioData.Actualizar(horario);
                
                if (resultado)
                {
                    return Json(new { success = true, message = "Horario actualizado exitosamente" });
                }
                return Json(new { success = false, message = "No se pudo actualizar el horario" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Horario/Eliminar
        [HttpPost]
        public async Task<IActionResult> Eliminar(int idHorario)
        {
            try
            {
                var resultado = await _horarioData.Eliminar(idHorario);
                
                if (resultado)
                {
                    return Json(new { success = true, message = "Horario eliminado exitosamente" });
                }
                return Json(new { success = false, message = "No se pudo eliminar el horario" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Horario/CambiarEstado
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int idHorario, bool activo)
        {
            try
            {
                var horario = await _horarioData.ObtenerPorId(idHorario);
                if (horario == null)
                {
                    return Json(new { success = false, message = "Horario no encontrado" });
                }

                horario.Activo = activo;
                var resultado = await _horarioData.Actualizar(horario);
                
                if (resultado)
                {
                    return Json(new { success = true, message = "Estado actualizado exitosamente" });
                }
                return Json(new { success = false, message = "No se pudo actualizar el estado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO para recibir datos desde JavaScript
    public class HorarioDto
    {
        public int IdHorario { get; set; }
        public int IdDoctor { get; set; }
        public string DiaSemana { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty; // "08:00"
        public string HoraFin { get; set; } = string.Empty;     // "12:00"
        public int DuracionCitaMinutos { get; set; }
        public bool Activo { get; set; }
    }
}
