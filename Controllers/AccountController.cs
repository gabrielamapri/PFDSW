using Microsoft.AspNetCore.Mvc;
using PoliclinicoWeb.Data;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioData _usuarioData;
        private readonly IPacienteData _pacienteData;
        private readonly IDoctorData _doctorData;

        public AccountController(IUsuarioData usuarioData, IPacienteData pacienteData, IDoctorData doctorData)
        {
            _usuarioData = usuarioData;
            _pacienteData = pacienteData;
            _doctorData = doctorData;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al dashboard
            if (HttpContext.Session.GetString("Usuario") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contraseña)
        {
            try
            {
                var usuario = await _usuarioData.ValidarUsuario(nombreUsuario, contraseña);

                if (usuario != null)
                {
                    // Guardar datos en sesión
                    HttpContext.Session.SetString("Usuario", usuario.NombreUsuario);
                    HttpContext.Session.SetString("TipoUsuario", usuario.TipoUsuario);
                    HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);

                    if (usuario.IdRelacionado.HasValue)
                    {
                        HttpContext.Session.SetInt32("IdRelacionado", usuario.IdRelacionado.Value);

                        // Obtener nombre completo según el tipo
                        if (usuario.TipoUsuario == "Doctor")
                        {
                            HttpContext.Session.SetInt32("IdDoctor", usuario.IdRelacionado.Value);
                            var doctor = await _doctorData.ObtenerPorId(usuario.IdRelacionado.Value);
                            if (doctor != null)
                            {
                                HttpContext.Session.SetString("NombreCompleto", doctor.NombreCompleto);
                            }
                        }
                        else if (usuario.TipoUsuario == "Paciente")
                        {
                            HttpContext.Session.SetInt32("IdPaciente", usuario.IdRelacionado.Value);
                            var paciente = await _pacienteData.ObtenerPorId(usuario.IdRelacionado.Value);
                            if (paciente != null)
                            {
                                HttpContext.Session.SetString("NombreCompleto", paciente.NombreCompleto);
                            }
                        }
                    }

                    TempData["SuccessMessage"] = $"¡Bienvenido, {usuario.NombreUsuario}!";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ErrorMessage = "Usuario o contraseña incorrectos";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al iniciar sesión: {ex.Message}";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "Sesión cerrada exitosamente";
            return RedirectToAction("Login");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
