using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Filters;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Repository;
using ProjetoEstagio.Services;
using System.Linq;
using System.IO;


namespace ProjetoEstagio.Controllers
{

    public class EstagiarioController : Controller
    {
        private readonly IEstagiarioService _estagiarioService;
        private readonly ISessao _sessao;


        public EstagiarioController(
            IEstagiarioService estagiarioService,
            ISessao sessao)
        {
            _estagiarioService = estagiarioService;
            _sessao = sessao;
        }


        [Autorizacao(Perfil.Admin)]
        public IActionResult ListaEstagiarios()
        {
            // Usa o serviço existente para buscar todos
            List<EstagiarioModel> estagiarios = _estagiarioService.ListarTodos();

            // Retorna a View "ListaEstagiarios.cshtml"
            return View(estagiarios);
        }


        public IActionResult Cadastrar()
        {
            return View(new EstagiarioCadastroViewModel());
        }


        [HttpPost]
        public IActionResult Cadastrar(EstagiarioCadastroViewModel viewModel)
        {
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
            return View(viewModel);
        }


        [Autorizacao(Perfil.Admin, Perfil.Estagiario)]
        [HttpGet]
        // [Autorizacao(Perfil.Admin, Perfil.Estagiario)] <--- Se estiver usando
        public IActionResult Editar(int id)
        {
            // 1. Verifica quem está logado
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null) return RedirectToAction("Index", "Login");

            // 2. Busca o estagiário alvo
            EstagiarioModel estagiarioAlvo = _estagiarioService.BuscarPorId(id);
            if (estagiarioAlvo == null)
            {
                TempData["MensagemErro"] = "Registro não encontrado.";

                // Se for Admin, volta pra lista. Se for Estagiário, volta pro painel.
                if (usuarioLogado.Perfil == Perfil.Admin)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Principal");
            }

            // --- CORREÇÃO DE SEGURANÇA (IDOR) ---
            // Se quem está logado é um Estagiário...
            if (usuarioLogado.Perfil == Perfil.Estagiario)
            {
                // ...ele só pode ver se o ID do usuário dele bater com o do estagiário alvo
                if (estagiarioAlvo.UsuarioId != usuarioLogado.Id)
                {
                    return RedirectToAction("Index", "Restrito"); // Bloqueia
                }
            }
            // -------------------------------------

            // Mapeia do Model para a ViewModel
            var viewModel = new EstagiarioEditarViewModel
            {
                Id = estagiarioAlvo.Id,
                UsuarioId = estagiarioAlvo.UsuarioId, // Importante para a senha!
                CPF = estagiarioAlvo.CPF,
                Nome = estagiarioAlvo.Nome,
                Email = estagiarioAlvo.Email,
                Telefone = estagiarioAlvo.Telefone,
                NomeCurso = estagiarioAlvo.NomeCurso
            };

            return View(viewModel);
        }


        [Autorizacao(Perfil.Admin, Perfil.Estagiario)]
        [HttpPost]
        public IActionResult Alterar(EstagiarioEditarViewModel viewModel)
        {
            try
            {
                // 1. Verifica quem está logado
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                if (usuarioLogado == null) return RedirectToAction("Index", "Login");

                // --- CORREÇÃO DE SEGURANÇA (IDOR) ---
                if (usuarioLogado.Perfil == Perfil.Estagiario)
                {
                    // Busca o estagiário que está tentando ser alterado
                    // (Poderíamos buscar pelo ID do usuário logado para ser mais seguro ainda,
                    // mas validar se o alvo pertence ao logado já resolve)
                    EstagiarioModel estagiarioAlvo = _estagiarioService.BuscarPorId(viewModel.Id);

                    if (estagiarioAlvo == null || estagiarioAlvo.UsuarioId != usuarioLogado.Id)
                    {
                        return RedirectToAction("Index", "Restrito"); // Bloqueia
                    }
                }
                // -------------------------------------

                if (ModelState.IsValid)
                {
                    EstagiarioModel estagiarioParaAtualizar = new EstagiarioModel
                    {
                        Id = viewModel.Id,
                        Nome = viewModel.Nome,
                        Email = viewModel.Email,
                        Telefone = viewModel.Telefone,
                        NomeCurso = viewModel.NomeCurso
                    };

                    _estagiarioService.Atualizar(estagiarioParaAtualizar);
                    TempData["MensagemSucesso"] = "Dados alterados com sucesso";

                    // Redireciona conforme o perfil
                    if (usuarioLogado.Perfil == Perfil.Admin) return RedirectToAction("Index");
                    return RedirectToAction("Principal");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} na alteração. Tente novamente";

                // Ajuste o redirecionamento de erro também
                if (_sessao.BuscarSessaoDoUsuario()?.Perfil == Perfil.Admin) return RedirectToAction("Index");
                return RedirectToAction("Principal");
            }

            return View("Editar", viewModel);
        }


        [Autorizacao(Perfil.Admin, Perfil.Estagiario)]
        public IActionResult DeletarConfirmar(int id)
        {
            EstagiarioModel estagiario = _estagiarioService.BuscarPorId(id);
            if (estagiario == null) return NotFound();
            return View(estagiario);
        }

        [Autorizacao(Perfil.Admin, Perfil.Estagiario)]
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

        [Autorizacao(Perfil.Estagiario)]
        public IActionResult Principal()
        {
            try
            {
                // 1. Buscar o Estagiário logado (lógica que você já tem)
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Estagiario)
                {
                    return RedirectToAction("Index", "Login");
                }

                EstagiarioModel estagiario = _estagiarioService.BuscarPorUsuarioId(usuarioLogado.Id);
                if (estagiario == null)
                {
                    TempData["MensagemErro"] = "Perfil de estagiário não encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                // 2. Chamar o serviço para listar as solicitações
                // (O serviço já foi atualizado no passo anterior)
                List<SolicitacaoEstagioModel> solicitacoes = _estagiarioService.ListarSolicitacoes(estagiario.Id);

                // 3. Montar o novo ViewModel
                var viewModel = new EstagiarioDashboardViewModel
                {
                    Solicitacoes = solicitacoes,

                    // Lógica para os cartões
                    ProcessosPendentesCount = solicitacoes.Count(s =>
                        s.Status == Status.Pendente ||
                        s.Status == Status.Aprovado),

                    // Lógica para o botão "Iniciar"
                    PossuiProcessoAtivoOuPendente = solicitacoes.Any(s =>
                        s.Status == Status.Pendente ||
                        s.Status == Status.Aprovado ||
                        s.Status == Status.EmAndamento),

                    // (Valores estáticos por enquanto)
                    RelatoriosPendentesCount = 0,
                    DocumentosPendentesCount = solicitacoes.Count(s =>
                        s.Status == Status.EmAndamento &&
                        s.TermoCompromisso?.CaminhoArquivo != null)
                };

                // 4. Enviar o NOVO ViewModel para a View
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar painel: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [Autorizacao(Perfil.Estagiario)]
        [HttpGet]
        public IActionResult MeuPerfil()
        {
            try
            {
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Estagiario)
                {
                    TempData["MensagemErro"] = "Acesso negado. Faça o login como Estagiário.";
                    return RedirectToAction("Index", "Login");
                }

                EstagiarioModel estagiario = _estagiarioService.BuscarPorUsuarioId(usuarioLogado.Id);
                if (estagiario == null)
                {
                    TempData["MensagemErro"] = "Nenhum perfil de estagiário encontrado para seu usuário.";
                    return RedirectToAction("Index", "Home");
                }

                // Redireciona para a Action Editar(id) existente
                return RedirectToAction("Editar", new { id = estagiario.Id });
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao localizar perfil: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Login() => View("Login");

        [Autorizacao(Perfil.Estagiario)]
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


        [Autorizacao(Perfil.Estagiario)]
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
                return RedirectToAction("Principal");
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

        [Autorizacao(Perfil.Estagiario)]
        [HttpGet]
        public IActionResult DownloadTermo(int termoId)
        {
            try
            {
                // 1. Buscar o Estagiário logado
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                EstagiarioModel estagiario = _estagiarioService.BuscarPorUsuarioId(usuarioLogado.Id);
                if (estagiario == null)
                {
                    return StatusCode(403, "Acesso negado.");
                }

                // 2. Chama o serviço (que contém a lógica de segurança)
                var resultado = _estagiarioService.PrepararDownloadTermo(termoId, estagiario.Id);

                // 3. Retorna o arquivo
                return File(resultado.FileContents, "application/pdf", resultado.NomeArquivo);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao baixar o arquivo: {ex.Message}";
                return RedirectToAction("Principal");
            }
        }


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
    }
}