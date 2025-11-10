// Controllers/OrientadorController.cs
using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Services;
using System.Threading.Tasks;

namespace ProjetoEstagio.Controllers
{
    public class OrientadorController : Controller
    {
        private readonly IOrientadorService _orientadorService;
        private readonly ISessao _sessao;

        public OrientadorController(IOrientadorService orientadorService, ISessao sessao)
        {
            _orientadorService = orientadorService;
            _sessao = sessao;
        }

        // Página principal para listar todos os orientadores
        public IActionResult Index()
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Apenas o Admin pode ver a lista de Orientadores
            if (usuarioLogado.Perfil != Perfil.Admin)
            {
                TempData["MensagemErro"] = "Você não tem permissão para acessar esta página.";
                return RedirectToAction("Index", "Home");
            }

            List<OrientadorModel> orientadores = _orientadorService.ListarTodos();
            return View(orientadores);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            // Retorna a Partial View para o modal
            return PartialView("_Cadastrar", new OrientadorCadastroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(OrientadorCadastroViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Apenas Admin pode cadastrar orientadores
                    UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                    if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
                    {
                        return StatusCode(403, "Acesso negado.");
                    }

                    _orientadorService.RegistrarNovoOrientador(viewModel);
                    return Json(new { sucesso = true });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erro ao cadastrar: {ex.Message}");
                }
            }

            // Retorna erros de validação para o AJAX
            return BadRequest(ModelState);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                return StatusCode(403, "Acesso negado.");
            }

            OrientadorModel orientador = _orientadorService.BuscarPorId(id);
            if (orientador == null)
                return NotFound();

            // Retorna a Partial View para o modal de edição
            return PartialView("_Editar", orientador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(OrientadorModel orientador)
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                return StatusCode(403, "Acesso negado.");
            }

            try
            {
                _orientadorService.AtualizarOrientador(orientador);
                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao editar: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Deletar(int id)
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                return StatusCode(403, "Acesso negado.");
            }

            OrientadorModel orientador = _orientadorService.BuscarPorId(id);
            if (orientador == null)
                return NotFound();

            // Retorna a Partial View para o modal de confirmação
            return PartialView("_Deletar", orientador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deletar(OrientadorModel orientador) // Recebe o modelo só para pegar o ID
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                return StatusCode(403, "Acesso negado.");
            }

            try
            {
                _orientadorService.DeletarOrientador(orientador.Id);
                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar: {ex.Message}");
            }
        }

        // MÉTODO DE VALIDAÇÃO [Remote]
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarCPFUnico(string cpf)
        {
            bool cpfJaExiste = await _orientadorService.VerificarCPFUnico(cpf);

            if (cpfJaExiste)
            {
                return Json($"O CPF {cpf} já está cadastrado.");
            }
            return Json(true);
        }
    }
}