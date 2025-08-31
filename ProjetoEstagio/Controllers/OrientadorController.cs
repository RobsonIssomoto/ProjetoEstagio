using Microsoft.AspNetCore.Mvc;

namespace ProjetoEstagio.Controllers
{
    public class OrientadorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cadastro()
        {
            return View("Cadastro");
        }

        public IActionResult Login()
        {
            return View("Login");
        }
    }
}
