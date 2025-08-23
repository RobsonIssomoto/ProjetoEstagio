using Microsoft.AspNetCore.Mvc;

namespace ProjetoEstagio.Controllers
{
    public class EstagiarioController : Controller
    {
        public IActionResult Index()
        {
            return View("Login");
        }
    }
}
