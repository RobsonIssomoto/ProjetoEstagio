using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; //Para SelectList
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Services; // Sua camada de serviço
using ProjetoEstagio.Helper; // Para ISessao
using System.Linq;         // Para .Count() 
using System.IO;

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
            var viewModel = new EmpresaDashBoardViewModel
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

            // --- MUDANÇA AQUI ---
            // Mapeia do Model para a ViewModel
            var viewModel = new EmpresaEditarViewModel
            {
                Id = empresa.Id,
                CNPJ = empresa.CNPJ,
                RazaoSocial = empresa.RazaoSocial,
                Nome = empresa.Nome,
                Email = empresa.Email,
                Telefone = empresa.Telefone
            };

            return View(viewModel); // Envia a ViewModel para a View
        }


        [HttpPost]
        public IActionResult Alterar(EmpresaEditarViewModel viewModel) // <-- MUDANÇA 1: Recebe a ViewModel
        {
            try
            {
                // MUDANÇA 2: Agora o ModelState.IsValid vai funcionar!
                if (ModelState.IsValid)
                {
                    // MUDANÇA 3: Mapeia da ViewModel para o Model
                    // (O Repositório só precisa dos campos que atualiza)
                    EmpresaModel empresaParaAtualizar = new EmpresaModel
                    {
                        Id = viewModel.Id,
                        RazaoSocial = viewModel.RazaoSocial,
                        Nome = viewModel.Nome,
                        Email = viewModel.Email,
                        Telefone = viewModel.Telefone
                    };

                    _empresaService.Atualizar(empresaParaAtualizar); //
                    TempData["MensagemSucesso"] = "Dados da empresa alterado com sucesso";
                    return RedirectToAction("Principal");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} na alteração dos dados da empresa. Tente novamente";
                return RedirectToAction("Principal");
            }

            // MUDANÇA 4: Se o ModelState for inválido, retorna a ViewModel
            // (Isso fará as mensagens de erro [Required] aparecerem)
            return View("Editar", viewModel);
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

        // Coloque este método dentro do seu EmpresaController.cs

        [HttpGet]
        public IActionResult MeuPerfil()
        {
            try
            {
                // 1. Busca o usuário logado na sessão
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();

                // 2. Valida se ele é um Representante (ou o perfil que pode editar)
                if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Representante)
                {
                    TempData["MensagemErro"] = "Acesso negado. Faça o login como Representante.";
                    return RedirectToAction("Index", "Login");
                }

                // 3. Busca a Empresa vinculada a este usuário
                EmpresaModel empresa = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id);

                if (empresa == null)
                {
                    TempData["MensagemErro"] = "Nenhuma empresa encontrada para o seu usuário.";
                    return RedirectToAction("Index", "Home"); // Ou para o Login
                }

                // 4. REDIRECIONA para a action Editar(id) existente, passando o ID
                return RedirectToAction("Editar", new { id = empresa.Id });
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao localizar perfil: {ex.Message}";
                return RedirectToAction("Index", "Home");
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
                // --- 1. BUSCA DE DADOS (JÁ EXISTENTE) ---
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                EmpresaModel empresaLogada = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id);
                if (usuarioLogado == null || empresaLogada == null)
                {
                    return StatusCode(403, "Sessão inválida.");
                }

                // --- 2. BUSCAR A LISTA DE SUPERVISORES (NOVO) ---
                // (Usando o método que o seu serviço já possui)
                var empresaComSupervisores = _empresaService.BuscarComSupervisores(empresaLogada.Id);
                var supervisoresDaEmpresa = empresaComSupervisores?.Supervisores ?? new List<SupervisorModel>();


                // --- 3. BUSCAR/CRIAR O TERMO (JÁ EXISTENTE) ---
                TermoCompromissoModel termo = _empresaService.BuscarOuCriarTermoPorSolicitacao(solicitacaoId, empresaLogada.Id);

                if (termo == null)
                {
                    return StatusCode(404, "Termo de Compromisso não encontrado.");
                }

                // --- 4. MAPEAMENTO ATUALIZADO PARA O VIEWMODEL ---
                var viewModel = new TermoPreenchimentoViewModel
                {
                    TermoId = termo.Id,
                    SolicitacaoId = termo.SolicitacaoEstagioId,

                    // Dados de exibição
                    EstagiarioNome = termo.SolicitacaoEstagio.Estagiario.Nome,
                    EmpresaNome = termo.SolicitacaoEstagio.Empresa.Nome,

                    // Dados do contrato (com valores padrão)
                    CargaHoraria = termo.CargaHoraria ?? 0,
                    ValorBolsa = termo.ValorBolsa ?? 0.0,
                    DataInicio = termo.DataInicio ?? DateTime.Now,
                    DataFim = termo.DataFim ?? DateTime.Now.AddMonths(6),
                    NumeroApolice = termo.NumeroApolice,
                    NomeSeguradora = termo.NomeSeguradora,
                    Justificativa = termo.Justificativa,

                    // Preenche o Plano de Atividades salvo
                    PlanoDeAtividades = termo.PlanoDeAtividades,

                    // Preenche o Supervisor salvo
                    SupervisorId = termo.SupervisorId,

                    // Cria o SelectList para o dropdown, já selecionando o ID salvo
                    SupervisoresDisponiveis = new SelectList(supervisoresDaEmpresa, "Id", "Nome", termo.SupervisorId)
                };
                // --- FIM DA ATUALIZAÇÃO ---

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
            // --- INÍCIO DA CORREÇÃO ---
            // Lógica de repopular em caso de erro
            Action<TermoPreenchimentoViewModel> repopularViewModel = (vm) => {
                try
                {
                    // Esta lógica é uma mini-versão do HttpGet PreencherTermo
                    var usuario = _sessao.BuscarSessaoDoUsuario();
                    var empresa = _empresaService.BuscarEmpresaPorUsuarioId(usuario.Id);

                    // 1. Recarregar a lista de Supervisores
                    var empresaComSupervisores = _empresaService.BuscarComSupervisores(empresa.Id);
                    var supervisores = empresaComSupervisores?.Supervisores ?? new List<SupervisorModel>();
                    vm.SupervisoresDisponiveis = new SelectList(supervisores, "Id", "Nome", vm.SupervisorId);

                    // 2. Recarregar dados do Termo (para os nomes readonly)
                    var termo = _empresaService.BuscarOuCriarTermoPorSolicitacao(vm.SolicitacaoId, empresa.Id);
                    vm.EstagiarioNome = termo.SolicitacaoEstagio.Estagiario.Nome;
                    vm.EmpresaNome = termo.SolicitacaoEstagio.Empresa.Nome;
                }
                catch (Exception ex)
                {
                    // Se até a repopulação falhar, apenas registre
                    Console.WriteLine($"Erro ao repopular ViewModel: {ex.Message}");
                }
            };
            // --- FIM DA CORREÇÃO ---

            if (!ModelState.IsValid)
            {
                // Agora esta linha vai funcionar e preencher o dropdown
                repopularViewModel(viewModel);
                return View("PreencherTermo", viewModel);
            }

            try
            {
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                EmpresaModel empresaLogada = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id);
                if (usuarioLogado == null || empresaLogada == null) return StatusCode(403, "Sessão inválida.");

                _empresaService.SalvarTermo(viewModel, empresaLogada.Id);

                TempData["MensagemSucesso"] = "Dados do contrato salvos e estágio aprovado com sucesso!";
                return RedirectToAction("Principal");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao salvar: {ex.Message}";
                // Também repopula aqui em caso de exceção
                repopularViewModel(viewModel);
                return View("PreencherTermo", viewModel);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejeitarTermo(TermoPreenchimentoViewModel viewModel)
        {
            // --- INÍCIO DA CORREÇÃO ---
            Action<TermoPreenchimentoViewModel> repopularViewModel = (vm) => {
                try
                {
                    var usuario = _sessao.BuscarSessaoDoUsuario();
                    var empresa = _empresaService.BuscarEmpresaPorUsuarioId(usuario.Id);

                    var empresaComSupervisores = _empresaService.BuscarComSupervisores(empresa.Id);
                    var supervisores = empresaComSupervisores?.Supervisores ?? new List<SupervisorModel>();
                    vm.SupervisoresDisponiveis = new SelectList(supervisores, "Id", "Nome", vm.SupervisorId);

                    var termo = _empresaService.BuscarOuCriarTermoPorSolicitacao(vm.SolicitacaoId, empresa.Id);
                    vm.EstagiarioNome = termo.SolicitacaoEstagio.Estagiario.Nome;
                    vm.EmpresaNome = termo.SolicitacaoEstagio.Empresa.Nome;
                }
                catch (Exception ex) { Console.WriteLine($"Erro ao repopular ViewModel: {ex.Message}"); }
            };
            // --- FIM DA CORREÇÃO ---

            try
            {
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                EmpresaModel empresaLogada = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id);
                if (usuarioLogado == null || empresaLogada == null) return StatusCode(403, "Sessão inválida.");

                _empresaService.RejeitarTermo(viewModel, empresaLogada.Id);

                TempData["MensagemSucesso"] = "Solicitação rejeitada com sucesso.";
                return RedirectToAction("Principal");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao rejeitar: {ex.Message}";

                // Recarrega a página em caso de erro
                repopularViewModel(viewModel);
                return View("PreencherTermo", viewModel);
            }
        }

        [HttpGet]
        public IActionResult DownloadTermo(int termoId)
        {
            try
            {
                // 1. Buscar a Empresa logada (lógica da sua Action Principal)
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                EmpresaModel empresa = _empresaService.BuscarEmpresaPorUsuarioId(usuarioLogado.Id);

                if (empresa == null || usuarioLogado.Perfil != Perfil.Representante)
                {
                    return StatusCode(403, "Acesso negado.");
                }

                // 2. Chama o serviço (que contém a lógica de segurança)
                var resultado = _empresaService.PrepararDownloadTermo(termoId, empresa.Id);

                // 3. Retorna o arquivo
                return File(resultado.FileContents, "application/pdf", resultado.NomeArquivo);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao baixar o arquivo: {ex.Message}";
                return RedirectToAction("Principal");
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