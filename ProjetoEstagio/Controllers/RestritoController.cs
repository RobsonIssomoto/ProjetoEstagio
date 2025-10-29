using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Filters;

namespace ProjetoEstagio.Controllers
{
  
    public class RestritoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
