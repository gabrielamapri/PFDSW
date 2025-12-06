using Microsoft.AspNetCore.Mvc;
using PoliclinicoWeb.Models;
using System.Diagnostics;

namespace PoliclinicoWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Usuario = usuario;
            ViewBag.TipoUsuario = HttpContext.Session.GetString("TipoUsuario");
            ViewBag.NombreCompleto = HttpContext.Session.GetString("NombreCompleto");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
