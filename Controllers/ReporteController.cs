using Microsoft.AspNetCore.Mvc;
using PoliclinicoWeb.Data;

namespace PoliclinicoWeb.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IReporteData _reporteData;

        public ReporteController(IReporteData reporteData)
        {
            _reporteData = reporteData;
        }

        public IActionResult Index()
        {
            // Verificar sesión
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public IActionResult CitasPorEspecialidad(string fechaInicio, string fechaFin)
        {
            try
            {
                var reporte = _reporteData.ObtenerCitasPorEspecialidad(fechaInicio, fechaFin);
                return Json(reporte);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult CitasPorDoctor(string fechaInicio, string fechaFin, string estado)
        {
            try
            {
                var reporte = _reporteData.ObtenerCitasPorDoctor(fechaInicio, fechaFin, estado);
                return Json(reporte);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult CitasPorEstado()
        {
            try
            {
                var reporte = _reporteData.ObtenerCitasPorEstado();
                return Json(reporte);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult PacientesFrecuentes(int top = 10)
        {
            try
            {
                var reporte = _reporteData.ObtenerPacientesFrecuentes(top);
                return Json(reporte);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
