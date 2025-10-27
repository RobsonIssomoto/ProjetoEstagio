using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models;
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{
    public class EmpresaController : Controller
    {
        public EmpresaController(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        private readonly IEmpresaRepository _empresaRepository;

        public IActionResult Index()
        {
            List<EmpresaModel> empresas = _empresaRepository.ListarTodos();
            return View(empresas);
        }

        public IActionResult Principal()
        {
            return View();
        }

        public IActionResult Cadastrar1()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar1(EmpresaModel empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _empresaRepository.Cadastrar(empresa);
                    TempData["MensagemSucesso"] = "Empresa cadastrada com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} no cadastro da empresa. Tente novamente";
                return RedirectToAction("Index");
            }
            return View(empresa);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            EmpresaModel empresa = _empresaRepository.BuscarPorId(id);

            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        [HttpPost]
        public IActionResult Alterar(EmpresaModel empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _empresaRepository.Atualizar(empresa);
                    TempData["MensagemSucesso"] = "Dados da empresa alterado com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} na alteração dos dados da empresa. Tente novamente";
                return RedirectToAction("Index");
            }
            return View("Editar", empresa);
        }

        public IActionResult DeletarConfirmar(int id)
        {
            EmpresaModel empresa = _empresaRepository.BuscarPorId(id);

            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }
        public IActionResult Deletar(int id)
        {
            try
            {
                bool deletar = _empresaRepository.Deletar(id);

                if (deletar)
                {
                    TempData["MensagemSucesso"] = "Empresa excluída com sucesso";

                }
                else
                {
                    TempData["MensagemErro"] = $"Erro ao excluir empresa. Tente novamente";
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

        public IActionResult DetalhesSupervisores(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Usamos o NOVO método do repositório
            EmpresaModel empresa = _empresaRepository.BuscarComSupervisores(id);

            if (empresa == null)
            {
                return NotFound("Empresa não encontrada.");
            }

            // Envia o objeto 'empresa' (que agora contém a lista
            // de supervisores) para a View.
            return View(empresa);
        }

        // ... (O resto dos seus métodos: Deletar, Login, etc.) ...
    }
}

