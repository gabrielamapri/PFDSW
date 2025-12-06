using Microsoft.AspNetCore.Mvc;
using PoliclinicoWeb.Data;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Controllers
{
    public class EspecialidadController : Controller
    {
        private readonly IEspecialidadData _especialidadData;

        public EspecialidadController(IEspecialidadData especialidadData)
        {
            _especialidadData = especialidadData;
        }

        // GET: Especialidad
        public IActionResult Index()
        {
            // Verificar autenticación
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Solo Admin puede acceder
            if (HttpContext.Session.GetString("TipoUsuario") != "Admin")
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var especialidades = await _especialidadData.ListarTodas();
            return Json(especialidades);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var especialidad = await _especialidadData.ObtenerPorId(id);
            if (especialidad == null)
            {
                return NotFound();
            }
            return Json(especialidad);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Especialidad especialidad)
        {
            try
            {
                if (especialidad.IdEspecialidad == 0)
                {
                    var resultado = await _especialidadData.Crear(especialidad);
                    return Json(new { success = resultado, message = "Especialidad creada exitosamente" });
                }
                else
                {
                    var resultado = await _especialidadData.Actualizar(especialidad);
                    return Json(new { success = resultado, message = "Especialidad actualizada exitosamente" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _especialidadData.Eliminar(id);
                return Json(new { success = resultado, message = "Especialidad eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Especialidad/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("TipoUsuario") != "Admin")
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }
            return View();
        }

        // POST: Especialidad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Especialidad especialidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _especialidadData.Crear(especialidad);
                    if (resultado)
                    {
                        TempData["SuccessMessage"] = "Especialidad creada exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                }
                ViewBag.ErrorMessage = "Error al crear la especialidad";
                return View(especialidad);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View(especialidad);
            }
        }

        // GET: Especialidad/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("TipoUsuario") != "Admin")
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            var especialidad = await _especialidadData.ObtenerPorId(id);
            if (especialidad == null)
            {
                return NotFound();
            }
            return View(especialidad);
        }

        // POST: Especialidad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Especialidad especialidad)
        {
            if (id != especialidad.IdEspecialidad)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _especialidadData.Actualizar(especialidad);
                    if (resultado)
                    {
                        TempData["SuccessMessage"] = "Especialidad actualizada exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                }
                ViewBag.ErrorMessage = "Error al actualizar la especialidad";
                return View(especialidad);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View(especialidad);
            }
        }

        // POST: Especialidad/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _especialidadData.Eliminar(id);
                if (resultado)
                {
                    return Json(new { success = true, message = "Especialidad eliminada exitosamente" });
                }
                return Json(new { success = false, message = "Error al eliminar la especialidad" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Obtener especialidades activas para select
        [HttpGet]
        public async Task<JsonResult> ObtenerEspecialidadesActivas()
        {
            var especialidades = await _especialidadData.ListarActivas();
            return Json(especialidades);
        }
    }
}
