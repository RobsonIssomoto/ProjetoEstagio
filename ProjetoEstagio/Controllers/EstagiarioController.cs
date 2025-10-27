using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{
    public class EstagiarioController : Controller
    {
        private readonly IEstagiarioRepository _estagiarioRepository;
        public EstagiarioController(IEstagiarioRepository estagiarioRepository)
        {
            _estagiarioRepository = estagiarioRepository;
        }

        public IActionResult Index()
        {
            List<EstagiarioModel> estagiario = _estagiarioRepository.ListarTodos();
            return View(estagiario);
        }

        public IActionResult Cadastrar1()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar1(EstagiarioModel estagiario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _estagiarioRepository.Cadastrar(estagiario);
                    TempData["MensagemSucesso"] = "Estagiário cadastrado com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} no cadastro do estagiario. Tente novamente";
                return RedirectToAction("Index");
            }
            return View(estagiario);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            EstagiarioModel estagiario = _estagiarioRepository.BuscarPorId(id);

            if (estagiario == null)
            {
                return NotFound();
            }

            return View(estagiario);
        }


        [HttpPost]
        public IActionResult Alterar(EstagiarioModel estagiario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _estagiarioRepository.Atualizar(estagiario);
                    TempData["MensagemSucesso"] = "Dados da empresa alterado com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} na alteração dos dados da empresa. Tente novamente";
                return RedirectToAction("Index");
            }
            return View("Editar", estagiario);
        }

        public IActionResult DeletarConfirmar(int id)
        {
            EstagiarioModel estagiario = _estagiarioRepository.BuscarPorId(id);

            if (estagiario == null)
            {
                return NotFound();
            }

            return View(estagiario);
        }
        public IActionResult Deletar(int id)
        {
            try
            {
                bool deletar = _estagiarioRepository.Deletar(id);

                if (deletar)
                {
                    TempData["MensagemSucesso"] = "Estagiário excluída com sucesso";

                }
                else
                {
                    TempData["MensagemErro"] = $"Erro ao excluir estagiário. Tente novamente";
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Devido erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Login()
        {
            return View("Login");
        }
    }
}
