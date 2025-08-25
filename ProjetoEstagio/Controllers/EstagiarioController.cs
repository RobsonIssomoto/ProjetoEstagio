using Microsoft.AspNetCore.Mvc;

namespace ProjetoEstagio.Controllers
{
    public class EstagiarioController : Controller
    {
        public IActionResult Index()
        {
            return View("../Home/Index");
        }

        public IActionResult Login()
        {
            return View("Login");
        }

        public IActionResult Cadastro()
        {
            return View("Cadastro");
        }

        public IActionResult Principal()
        {
            return View("Principal");
        }



    }
}
