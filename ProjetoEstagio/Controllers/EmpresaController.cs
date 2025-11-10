using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Services; // Sua camada de serviço
using ProjetoEstagio.Helper; // Para ISessao
using System.Linq;         // Para .Count()

namespace ProjetoEstagio.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IEmpresaService _empresaService;
        private readonly ISessao _sessao; // Injetado para o dashboard

        public EmpresaController(
            IEmpresaService empresaService,
            ISessao sessao) // Context removido, Sessao adicionada
        {
            _empresaService = empresaService;
            _sessao = sessao;
        }

        // --- LÓGICA DE CADASTRO PÚBLICO ---

        // GET: /Empresa/Cadastrar?token=...
        public IActionResult Cadastrar(string? token) // Aceita o token da URL
        {
            var viewModel = new EmpresaCadastroViewModel();

            // (Opcional, mas recomendado) Se o token existir, armazene-o
            // para que o POST possa recebê-lo de volta.
            if (token != null)
            {
                ViewData["Token"] = token;
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Cadastrar(EmpresaCadastroViewModel viewModel, string? token) // Recebe o token
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Envia o token para o serviço
                    _empresaService.RegistrarNovaEmpresa(viewModel, token);

                    TempData["MensagemSucesso"] = "Empresa cadastrada! Faça o login.";
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar: {erro.Message}";
            }
            ViewData["Token"] = token; // Devolve o token para a view se houver erro
            return View(viewModel);
        }


        // --- DASHBOARD DO REPRESENTANTE ---

        // GET: /Empresa/Principal
        public IActionResult Principal()
        {
            // 1. Obter o usuário (Representante) logado
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Representante)
            {
                return RedirectToAction("Index", "Login");
            }

            // 2. Encontrar a Empresa vinculada (via Serviço)
            EmpresaModel empresa = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id); //
            if (empresa == null)
            {
                TempData["MensagemErro"] = "Erro: Empresa não encontrada para este usuário.";
                return RedirectToAction("Index", "Login");
            }

            // (Opcional: Salvar o EmpresaId na sessão para o SupervisorController)
            // _sessao.SalvarEmpresaIdNaSessao(empresa.Id);

            // 3. Buscar as solicitações (via Serviço)
            var solicitacoes = _empresaService.ListarSolicitacoes(empresa.Id); //

            // 4. Montar o ViewModel para a View
            var viewModel = new DashboardEmpresaViewModel
            {
                PedidosPendentesCount = solicitacoes.Count(s => s.Status == Status.Pendente),
                PedidosDeEstagio = solicitacoes
            };

            // 5. Enviar o ViewModel para a View "Principal.cshtml"
            return View(viewModel);
        }


        // --- MÉTODOS DE ADMIN (CRUD) ---

        // GET: /Empresa/Index (Lista de Empresas para o Admin)
        public IActionResult Index()
        {
            List<EmpresaModel> empresas = _empresaService.ListarTodos(); //
            return View(empresas);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            EmpresaModel empresa = _empresaService.BuscarPorId(id); //
            if (empresa == null) return NotFound();
            return View(empresa);
        }


        [HttpPost]
        public IActionResult Alterar(EmpresaModel empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _empresaService.Atualizar(empresa); //
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
            EmpresaModel empresa = _empresaService.BuscarPorId(id); //
            if (empresa == null) return NotFound();
            return View(empresa);
        }


        public IActionResult Deletar(int id)
        {
            try
            {
                bool deletar = _empresaService.Deletar(id); //
                if (deletar) TempData["MensagemSucesso"] = "Empresa excluída com sucesso";
                else TempData["MensagemErro"] = $"Erro ao excluir empresa. Tente novamente";
                return RedirectToAction("Index");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Devido erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult DetalhesSupervisores(int id)
        {
            if (id == 0) return NotFound();
            EmpresaModel empresa = _empresaService.BuscarComSupervisores(id); //
            if (empresa == null) return NotFound("Empresa não encontrada.");
            return View(empresa);
        }

        
        [HttpGet]
        public async Task<IActionResult> BuscarEmpresas(string termoDeBusca)
        {
            if (string.IsNullOrEmpty(termoDeBusca))
            {
                return Json(new List<EmpresaModel>());
            }

            // Busca no serviço (presumindo que ListarTodos() busca no repositório)
            // Para otimizar, crie um método "BuscarPorNome(termo)" no seu service/repositório
            var empresas = _empresaService.ListarTodos()
                .Where(e => e.Nome.Contains(termoDeBusca, StringComparison.OrdinalIgnoreCase) ||
                            e.CNPJ.Contains(termoDeBusca))
                .Select(e => new { e.Id, e.Nome, e.CNPJ }) // Retorna só os dados necessários
                .Take(5) // Limita a 5 resultados
                .ToList();

            return Json(empresas);
        }

        // Em EmpresaController.cs
        [HttpGet]
        public IActionResult PreencherTermo(int solicitacaoId)
        {
            try
            {
                // --- INÍCIO DA VERIFICAÇÃO E CRIAÇÃO ---
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                EmpresaModel empresaLogada = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id);
                if (usuarioLogado == null || empresaLogada == null)
                {
                    return StatusCode(403, "Sessão inválida.");
                }

                // Esta linha busca ou cria o termo, e já o retorna com todos os Includes
                TermoCompromissoModel termo = _empresaService.BuscarOuCriarTermoPorSolicitacao(solicitacaoId, empresaLogada.Id);

                if (termo == null)
                {
                    return StatusCode(404, "Termo de Compromisso não encontrado.");
                }
                // --- FIM DA VERIFICAÇÃO E CRIAÇÃO ---


                // --- CORREÇÃO AQUI: Mapeamento do Model para o ViewModel ---
                var viewModel = new TermoPreenchimentoViewModel
                {
                    TermoId = termo.Id,
                    SolicitacaoId = termo.SolicitacaoEstagioId,

                    // Os dados de Aluno e Empresa agora vão funcionar
                    EstagiarioNome = termo.SolicitacaoEstagio.Estagiario.Nome,
                    EmpresaNome = termo.SolicitacaoEstagio.Empresa.Nome,

                    // A lógica das datas padrão também vai funcionar
                    // (Assumindo que o seu TermoCompromissoModel.cs tem 'DateTime?')
                    CargaHoraria = termo.CargaHoraria ?? 0,
                    ValorBolsa = termo.ValorBolsa ?? 0.0,
                    DataInicio = termo.DataInicio ?? DateTime.Now,
                    DataFim = termo.DataFim ?? DateTime.Now.AddMonths(6),
                    NumeroApolice = termo.NumeroApolice,
                    NomeSeguradora = termo.NomeSeguradora,
                    Justificativa = termo.Justificativa
                };
                // --- FIM DA CORREÇÃO ---

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar página: {ex.Message}";
                return RedirectToAction("Principal");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SalvarTermo(TermoPreenchimentoViewModel viewModel)
        {
            // Lógica de repopular em caso de erro
            Action<TermoPreenchimentoViewModel> repopularViewModel = (vm) => { /* ... (código anterior) ... */ };

            // A validação [Required] dos campos do ViewModel
            // garante que CargaHoraria, ValorBolsa, etc., foram preenchidos
            if (!ModelState.IsValid)
            {
                repopularViewModel(viewModel);
                return View("PreencherTermo", viewModel);
            }

            try
            {
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                EmpresaModel empresaLogada = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id);
                if (usuarioLogado == null || empresaLogada == null) return StatusCode(403, "Sessão inválida.");

                // Chama o novo serviço de "Salvar"
                _empresaService.SalvarTermo(viewModel, empresaLogada.Id);

                TempData["MensagemSucesso"] = "Dados do contrato salvos e estágio aprovado com sucesso!";
                return RedirectToAction("Principal");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao salvar: {ex.Message}";
                repopularViewModel(viewModel);
                return View("PreencherTermo", viewModel);
            }
        }


        // --- ADICIONE ESTE MÉTODO (para o botão "Rejeitar") ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejeitarTermo(TermoPreenchimentoViewModel viewModel)
        {
            try
            {
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                EmpresaModel empresaLogada = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id);
                if (usuarioLogado == null || empresaLogada == null) return StatusCode(403, "Sessão inválida.");

                // Chama o novo serviço de "Rejeitar"
                _empresaService.RejeitarTermo(viewModel, empresaLogada.Id);

                TempData["MensagemSucesso"] = "Solicitação rejeitada com sucesso.";
                return RedirectToAction("Principal");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao rejeitar: {ex.Message}";

                // Recarrega a página em caso de erro
                Action<TermoPreenchimentoViewModel> repopularViewModel = (vm) => { /* ... (código anterior) ... */ };
                repopularViewModel(viewModel);
                return View("PreencherTermo", viewModel);
            }
        }
        // --- OUTROS MÉTODOS ---

        public IActionResult Login() => View("Login");

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarCNPJUnico(string cnpj)
        {
            bool cnpjJaExiste = await _empresaService.VerificarCNPJUnico(cnpj); //
            if (cnpjJaExiste)
            {
                return Json("Este CNPJ já está cadastrado.");
            }
            return Json(true);
        }
    }
}