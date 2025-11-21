
using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting; 
using ProjetoEstagio.Repository;  
using System.IO;
using ProjetoEstagio.Filters;             

namespace ProjetoEstagio.Controllers
{
    
    public class OrientadorController : Controller
    {
        private readonly IOrientadorService _orientadorService;
        private readonly ISessao _sessao;
        private readonly ITermoCompromissoRepository _termoRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrientadorController(
            IOrientadorService orientadorService, 
            ISessao sessao, 
            ITermoCompromissoRepository termoRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _orientadorService = orientadorService;
            _sessao = sessao;
            _termoRepository = termoRepository;
            _webHostEnvironment = webHostEnvironment;
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

        // GET: /Orientador/Pendencias
        [HttpGet]
        public IActionResult Pendencias()
        {
            // 1. Segurança: Reutiliza a mesma lógica do seu Index
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                TempData["MensagemErro"] = "Você não tem permissão para acessar esta página.";
                return RedirectToAction("Index", "Home");
            }

            // 2. Busca os dados usando os serviços
            var termosPendentes = _orientadorService.ListarTermosPendentesDeOrientador();
            var orientadores = _orientadorService.ListarTodos(); //

            // 3. Monta o ViewModel
            var viewModel = new PendenciasOrientadorViewModel
            {
                TermosPendentes = termosPendentes,

                // Cria o SelectList para o dropdown
                OrientadoresDisponiveis = new SelectList(orientadores, "Id", "Nome")
            };

            // 4. Envia o ViewModel para a nova View (que criaremos a seguir)
            return View(viewModel);
        }

        // GET: /Orientador/Estagios
        [HttpGet]
        public IActionResult Estagios()
        {
            // 1. Segurança
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                TempData["MensagemErro"] = "Você não tem permissão para acessar esta página.";
                return RedirectToAction("Index", "Home");
            }

            // 2. Chama o novo método do serviço
            List<TermoCompromissoModel> todosOsTermos = _orientadorService.ListarTodosOsTermos();

            // 3. Envia a lista para a nova View
            return View(todosOsTermos);
        }

        // POST: /Orientador/Atribuir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Atribuir(PendenciasOrientadorViewModel viewModel)
        {
            // 1. Segurança
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                return StatusCode(403, "Acesso negado.");
            }

            try
            {
                // 2. Validação básica
                if (viewModel.TermoIdSelecionado <= 0 || viewModel.OrientadorIdSelecionado <= 0)
                {
                    throw new Exception("Seleção inválida.");
                }

                // 3. Chama o serviço que implementamos no passo anterior
                _orientadorService.AtribuirOrientador(
                    viewModel.TermoIdSelecionado,
                    viewModel.OrientadorIdSelecionado
                );

                TempData["MensagemSucesso"] = "Orientador atribuído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao atribuir: {ex.Message}";
            }

            // 4. Retorna para a página de pendências
            return RedirectToAction("Pendencias");
        }

        [HttpGet]
        public IActionResult DownloadTermo(int termoId)
        {
            try
            {
                // 1. Busca o termo no banco para pegar os nomes dos arquivos
                var termo = _termoRepository.BuscarPorId(termoId);

                // 2. Validações de segurança
                if (termo == null || string.IsNullOrEmpty(termo.CaminhoArquivo))
                {
                    return NotFound("Arquivo não encontrado ou o termo não existe.");
                }

                // (Opcional: Adicionar verificação de segurança se o Admin pode baixar isso)

                // 3. Monta o caminho absoluto do arquivo
                // (Ex: C:\seuprojeto\wwwroot\arquivos\termos\TCE_Joao_26.pdf)
                string caminhoAbsoluto = Path.Combine(_webHostEnvironment.WebRootPath, termo.CaminhoArquivo.TrimStart(Path.DirectorySeparatorChar));

                if (!System.IO.File.Exists(caminhoAbsoluto))
                {
                    TempData["MensagemErro"] = "Erro: O arquivo não foi encontrado no servidor.";
                    return RedirectToAction("Estagios");
                }

                // 4. Lê os bytes do arquivo e envia para o navegador
                byte[] fileBytes = System.IO.File.ReadAllBytes(caminhoAbsoluto);

                // Retorna o arquivo para download
                return File(fileBytes, "application/pdf", termo.NomeArquivo);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao baixar o arquivo: {ex.Message}";
                return RedirectToAction("Estagios");
            }
        }

        [HttpGet]
        public IActionResult AlterarOrientador(int id) // Recebe o TermoId
        {
            // 1. Segurança (só Admin)
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                return StatusCode(403, "Acesso negado.");
            }

            // 2. Busca os dados
            // (O _termoRepository foi injetado no construtor)
            var termo = _termoRepository.BuscarCompletoPorId(id);
            if (termo == null) return NotFound();

            var orientadores = _orientadorService.ListarTodos();

            // 3. Monta o ViewModel com o nome correto
            var viewModel = new OrientadorAlterarViewModel
            {
                TermoId = termo.Id,
                EstagiarioNome = termo.SolicitacaoEstagio.Estagiario.Nome,
                NovoOrientadorId = termo.OrientadorId ?? 0,
                //OrientadorAtualNome = termo.Orientador?.Nome ?? "Nenhum",
                OrientadoresDisponiveis = new SelectList(orientadores, "Id", "Nome", termo.OrientadorId)
            };

            // 4. Retorna a Partial View (que criaremos a seguir)
            return PartialView("_AlterarOrientador", viewModel);
        }

        // POST: /Orientador/AlterarOrientador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlterarOrientador(OrientadorAlterarViewModel viewModel) // Recebe o ViewModel correto
        {
            // 1. Segurança (só Admin)
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                return StatusCode(403, "Acesso negado.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 2. Chama o serviço que criamos
                    _orientadorService.AlterarOrientadorDoTermo(viewModel.TermoId, viewModel.NovoOrientadorId);
                    return Json(new { sucesso = true });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erro ao alterar: {ex.Message}");
                }
            }

            // Se o ModelState for inválido (ex: não selecionou um novo orientador)
            return BadRequest(ModelState);
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