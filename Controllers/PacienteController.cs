using Microsoft.AspNetCore.Mvc;
using PoliclinicoWeb.Data;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Controllers
{
    public class PacienteController : Controller
    {
        private readonly IPacienteData _pacienteData;
        private readonly ILogger<PacienteController> _logger;

        public PacienteController(IPacienteData pacienteData, ILogger<PacienteController> logger)
        {
            _pacienteData = pacienteData;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Verificar sesión
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Account");
            }

            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");
            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var pacientes = await _pacienteData.ListarTodos();
                return Json(pacientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar pacientes");
                // Devuelve un arreglo vacío para que DataTables no muestre 'Ajax error'
                return Json(new object[0]);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var paciente = await _pacienteData.ObtenerPorId(id);
            if (paciente == null)
            {
                return Json(new { success = false, message = "Paciente no encontrado" });
            }
            return Json(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Paciente paciente)
        {
            try
            {
                bool resultado;
                if (paciente.IdPaciente == 0)
                {
                    resultado = await _pacienteData.Crear(paciente);
                }
                else
                {
                    resultado = await _pacienteData.Actualizar(paciente);
                }

                if (resultado)
                {
                    return Json(new { success = true, message = "Operación realizada correctamente" });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo guardar el paciente" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Paciente paciente)
        {
            try
            {
                await _pacienteData.Crear(paciente);
                TempData["SuccessMessage"] = "Paciente creado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al crear el paciente: " + ex.Message;
                return View(paciente);
            }
        }

        public async Task<IActionResult> Editar(int id)
        {
            var paciente = await _pacienteData.ObtenerPorId(id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Paciente paciente)
        {
            try
            {
                await _pacienteData.Actualizar(paciente);
                TempData["SuccessMessage"] = "Paciente actualizado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar el paciente: " + ex.Message;
                return View(paciente);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _pacienteData.Eliminar(id);
                return Json(new { success = true, message = "Paciente eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }
    }
}
