using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Filters;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels; // <-- Adicionado
using ProjetoEstagio.Repository;
using ProjetoEstagio.Services;


namespace ProjetoEstagio.Controllers
{
    public class EstagiarioController : Controller
    {
        private readonly IEstagiarioService _estagiarioService;
        private readonly ISessao _sessao;
        // Removidas as injeções de repositório

        public EstagiarioController(
            IEstagiarioService estagiarioService,
            ISessao sessao)
        {
            _estagiarioService = estagiarioService;
            _sessao = sessao;
        }

        public IActionResult Index()
        {
            List<EstagiarioModel> estagiario = _estagiarioService.ListarTodos();
            return View(estagiario);
        }

        // 4. Ação GET atualizada para usar o ViewModel
        public IActionResult Cadastrar()
        {
            return View(new EstagiarioCadastroViewModel());
        }

        // 5. Ação POST substituída pela lógica do "Passo 5"
        [HttpPost]
        public IActionResult Cadastrar(EstagiarioCadastroViewModel viewModel)
        {
            // Começa a transação (ou salva os dois, ou falha os dois)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _estagiarioService.RegistrarNovoEstagiario(viewModel);
                        TempData["MensagemSucesso"] = "Cadastro realizado! Faça o login.";
                        return RedirectToAction("Index", "Login"); // Redireciona para o Login
                    }
                }
                catch (Exception erro)
                {
                    TempData["MensagemErro"] = $"Erro ao cadastrar: {erro.Message}";
                }
            }

            // Se o ModelState for inválido ou a transação falhar,
            // retorna para a view com os dados que o usuário digitou.
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            EstagiarioModel estagiario = _estagiarioService.BuscarPorId(id);
            if (estagiario == null) return NotFound();
            return View(estagiario);
        }

        [HttpPost]
        public IActionResult Alterar(EstagiarioModel estagiario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _estagiarioService.Atualizar(estagiario);
                    TempData["MensagemSucesso"] = "Dados alterados com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} na alteração. Tente novamente";
                return RedirectToAction("Index");
            }
            return View("Alterar", estagiario);
        }

        public IActionResult DeletarConfirmar(int id)
        {
            EstagiarioModel estagiario = _estagiarioService.BuscarPorId(id);
            if (estagiario == null) return NotFound();
            return View(estagiario);
        }

        public IActionResult Deletar(int id)
        {
            try
            {
                bool deletar = _estagiarioService.Deletar(id);
                if (deletar)
                    TempData["MensagemSucesso"] = "Estagiário excluído com sucesso";
                else
                    TempData["MensagemErro"] = $"Erro ao excluir estagiário. Tente novamente";
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Devido erro: {erro.Message}";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Principal()
        {
            return View();

        }

        /// Método [Remote] para validar CPF
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarCPFUnico(string cpf)
        {
            // Opcional, mas recomendado: Limpar a formatação do CPF (pontos e traços)
            // var cpfLimpo = cpf.Replace(".", "").Replace("-", "");

            bool cpfJaExiste = await _estagiarioService.VerificarCPFUnico(cpf);

            if (cpfJaExiste)
            {
                return Json($"O CPF {cpf} já está cadastrado.");
            }

            return Json(true);
        }

        public IActionResult Login() => View("Login");

        //
        // GET: /Estagiario/Processo
        //
        [HttpGet]
        public IActionResult Processo()
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Usa o SERVIÇO para buscar o estagiário
            var estagiario = _estagiarioService.BuscarPorUsuarioId(usuario.Id); //
            if (estagiario == null)
            {
                TempData["MensagemErro"] = "Perfil de estagiário não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new SolicitacaoCadastroViewModel
            {
                EstagiarioId = estagiario.Id,
                EstagiarioNome = estagiario.Nome,
                EstagiarioCurso = estagiario.NomeCurso
            };

            return View(viewModel);
        }

        //
        // POST: /Estagiario/CriarSolicitacao
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarSolicitacao(SolicitacaoCadastroViewModel viewModel)
        {
            // Busca os dados do estagiário (Nome/Curso) para recarregar a tela em caso de erro
            EstagiarioModel estagiario = null;
            try
            {
                if (!ModelState.IsValid)
                {
                    // Se o modelo não for válido, força a recarga da view
                    throw new InvalidOperationException("ModelState inválido.");
                }

                // Chama o SERVIÇO para fazer todo o trabalho
                _estagiarioService.CriarSolicitacao(viewModel);

                TempData["MensagemSucesso"] = "Solicitação de estágio enviada com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Se o serviço falhar (ex: "Selecione uma empresa"),
                // a exceção é capturada aqui.
                TempData["MensagemErro"] = $"Ocorreu um erro: {ex.Message}";

                // Recarrega os dados do estagiário para a view
                estagiario = _estagiarioService.BuscarPorId(viewModel.EstagiarioId); //
                viewModel.EstagiarioNome = estagiario.Nome;
                viewModel.EstagiarioCurso = estagiario.NomeCurso;
                return View("Processo", viewModel);
            }
        }
    }
}