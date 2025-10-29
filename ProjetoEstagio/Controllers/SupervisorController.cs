using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ISupervisorRepository _supervisorRepository;
        public SupervisorController(ISupervisorRepository supervisorRepository)
        {
            _supervisorRepository = supervisorRepository;
        }

        public IActionResult Index()
        {
            List<SupervisorModel> supervisores = _supervisorRepository.ListarTodos();
            return View(supervisores);
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        public IActionResult Principal()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(SupervisorModel supervisor)
        {
            _supervisorRepository.Cadastrar(supervisor);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            SupervisorModel supervisor = _supervisorRepository.BuscarPorId(id);
            if(supervisor == null)
            {
                return NotFound();
            }
            return View(supervisor);
        }

        [HttpPost]
        public IActionResult Editar(SupervisorModel supervisor)
        {
            if (ModelState.IsValid)
            {
                _supervisorRepository.Editar(supervisor);
                return RedirectToAction("Index");
            }
            return View(supervisor);
        }

        public IActionResult Alterar(SupervisorModel supervisor)
        {
            if (ModelState.IsValid)
            {
                _supervisorRepository.Editar(supervisor);
                return RedirectToAction("Index");
            }
            return View(supervisor);
        }

        public IActionResult DeletarConfirmar(int id)
        {
            SupervisorModel supervisor = _supervisorRepository.BuscarPorId(id);

            if (supervisor == null)
            {
                return NotFound();
            }
            return View(supervisor);
        }

        public IActionResult Deletar(int id)
        {
            _supervisorRepository.Deletar(id);
            return RedirectToAction("Index");
        }


        public IActionResult Login()
        {
            return View("Login");
        }
    }
}
