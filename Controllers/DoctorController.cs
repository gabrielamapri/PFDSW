using Microsoft.AspNetCore.Mvc;
using PoliclinicoWeb.Data;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorData _doctorData;
        private readonly IEspecialidadData _especialidadData;

        public DoctorController(IDoctorData doctorData, IEspecialidadData especialidadData)
        {
            _doctorData = doctorData;
            _especialidadData = especialidadData;
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
            var doctores = await _doctorData.ListarTodos();
            return Json(doctores);
        }

        public async Task<IActionResult> Crear()
        {
            ViewBag.Especialidades = await _especialidadData.ListarTodas();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Doctor doctor)
        {
            try
            {
                await _doctorData.Crear(doctor);
                TempData["SuccessMessage"] = "Doctor creado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al crear el doctor: " + ex.Message;
                ViewBag.Especialidades = await _especialidadData.ListarTodas();
                return View(doctor);
            }
        }

        public async Task<IActionResult> Editar(int id)
        {
            var doctor = await _doctorData.ObtenerPorId(id);
            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.Especialidades = await _especialidadData.ListarTodas();
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Doctor doctor)
        {
            try
            {
                await _doctorData.Actualizar(doctor);
                TempData["SuccessMessage"] = "Doctor actualizado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar el doctor: " + ex.Message;
                ViewBag.Especialidades = await _especialidadData.ListarTodas();
                return View(doctor);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _doctorData.Eliminar(id);
                return Json(new { success = true, message = "Doctor eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDoctoresPorEspecialidad(int idEspecialidad)
        {
            var doctores = await _doctorData.ListarPorEspecialidad(idEspecialidad);
            return Json(doctores);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var doctor = await _doctorData.ObtenerPorId(id);
            if (doctor == null)
            {
                return Json(new { success = false, message = "Doctor no encontrado" });
            }
            return Json(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Doctor doctor)
        {
            try
            {
                bool resultado;
                if (doctor.IdDoctor == 0)
                {
                    resultado = await _doctorData.Crear(doctor);
                }
                else
                {
                    resultado = await _doctorData.Actualizar(doctor);
                }

                if (resultado)
                {
                    return Json(new { success = true, message = "Operación realizada correctamente" });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo guardar el doctor" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
