using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Repository;
using ProjetoEstagio.Services;

namespace ProjetoEstagio.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ISupervisorService _supervisorService;
        private readonly ISessao _sessao;

        public SupervisorController(
            ISupervisorService supervisorService,
            ISessao sessao)
        {
            _supervisorService = supervisorService;
            _sessao = sessao;
        }
        public IActionResult Index()
        {
            // Busca o usuário logado
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            // Se não estiver logado, manda para o Login
            if (usuarioLogado == null)
            {
                return RedirectToAction("Index", "Login");
            }

            List<SupervisorModel> supervisores;

            // --- LÓGICA DE FILTRO POR PERFIL ---

            if (usuarioLogado.Perfil == Perfil.Admin)
            {
                // Se for Admin, lista todos de todas as empresas
                supervisores = _supervisorService.ListarTodos();
            }
            else if (usuarioLogado.Perfil == Perfil.Representante)
            {
                // Se for Representante, busca o ID da empresa salvo na sessão
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();

                // Checagem de segurança: e se o ID não estiver lá?
                if (empresaId == null)
                {
                    // Isso não deveria acontecer se o Login (Passo 3) foi feito
                    TempData["MensagemErro"] = "Erro ao recuperar informações da empresa. Tente fazer login novamente.";
                    return RedirectToAction("Index", "Home");
                }

                // Busca apenas os supervisores daquela empresa
                supervisores = _supervisorService.ListarPorEmpresa(empresaId.Value);
            }
            else
            {
                // Qualquer outro perfil (ex: Estagiário) não pode ver esta página
                TempData["MensagemErro"] = "Você não tem permissão para acessar esta página.";
                return RedirectToAction("Index", "Home");
            }

            // Retorna a View com a lista (ou vazia ou completa, dependendo do filtro)
            return View(supervisores);
        }

        public IActionResult Principal()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            // Retorna a Partial View com um ViewModel VAZIO
            return PartialView("_Cadastrar", new SupervisorCadastroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(SupervisorCadastroViewModel viewModel)
        {
            // O "Modo Nuclear" foi removido. Agora validamos!
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Buscar o ID da Empresa que está logada
                    int? empresaId = _sessao.BuscarEmpresaIdDaSessao();
                    if (empresaId == null)
                    {
                        return StatusCode(403, "Sessão inválida ou sem permissão.");
                    }

                    // 2. Chamar o Serviço para fazer o trabalho pesado
                    _supervisorService.RegistrarNovoSupervisor(viewModel, empresaId.Value);

                    // 3. Retornar sucesso (seu código AJAX espera isso)
                    return Json(new { sucesso = true });
                }
                catch (Exception ex)
                {
                    // Captura erros do serviço (ex: e-mail duplicado)
                    return StatusCode(500, $"Erro ao cadastrar: {ex.Message}");
                }
            }

            // Se o ModelState for inválido (ex: senhas não conferem),
            // retorna um erro 400 (Bad Request) com as mensagens de validação.
            // O AJAX deve ser capaz de ler isso.
            return BadRequest(ModelState);

        }


        [HttpGet]
        public IActionResult Editar(int id)
        {
            // --- INÍCIO DA VERIFICAÇÃO DE SEGURANÇA ---
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                // MUDANÇA AQUI:
                // return RedirectToAction("Index", "Login"); // <-- Linha antiga
                return StatusCode(401, "Sessão expirada. Faça login novamente."); // <-- Linha nova
            }

            SupervisorModel supervisor = _supervisorService.BuscarPorId(id);
            if (supervisor == null)
                return NotFound();

            if (usuarioLogado.Perfil == Perfil.Representante)
            {
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();

                if (supervisor.EmpresaId != empresaId.Value)
                {
                    TempData["MensagemErro"] = "Acesso negado. Este supervisor não pertence à sua empresa.";
                    // MUDANÇA OPCIONAL, MAS RECOMENDADA:
                    // return RedirectToAction("Index"); // <-- Linha antiga
                    return StatusCode(403, "Acesso negado."); // <-- Linha nova (Forbidden)
                }
            }

            // Para todos os perfis válidos (Admin ou Representante dono), retorna a Partial View
            return PartialView("_Editar", supervisor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(SupervisorModel supervisor)
        {
            Console.WriteLine("===== INÍCIO DO MÉTODO EDITAR ====="); // O "Modo Nuclear" não é mais necessário

            // --- 1. VERIFICAÇÃO DE SESSÃO ---
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                return StatusCode(401, "Sessão expirada. Faça login novamente.");
            }
            // ... (Verificação de permissão) ...
            if (usuarioLogado.Perfil == Perfil.Representante)
            {
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();
                if (supervisor.EmpresaId != empresaId.Value)
                {
                    return StatusCode(403, "Acesso negado.");
                }
            }

            // --- 2. CHAMADA AO SERVIÇO (AO INVÉS DO REPOSITÓRIO) ---
            try
            {
                // _supervisorRepository.Atualizar(supervisor); // <-- Linha antiga
                _supervisorService.AtualizarSupervisor(supervisor); // <-- NOVA LINHA

                Console.WriteLine("Edição realizada com SUCESSO!");
                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO EDITAR: {ex.Message}");
                return StatusCode(500, $"Erro de banco de dados: {ex.Message}");
            }
        }

        public IActionResult Alterar(SupervisorModel supervisor)
        {
            if (ModelState.IsValid)
            {
                _supervisorService.AtualizarSupervisor(supervisor);
                return RedirectToAction("Index");
            }
            return View(supervisor);
        }

        [HttpGet]
        public IActionResult Deletar(int id)
        {
            Console.WriteLine($"===== INÍCIO DO MÉTODO GET DELETAR (ID: {id}) =====");

            // --- Verificação de Sessão ---
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                return StatusCode(401, "Sessão expirada. Faça login novamente.");
            }

            // --- Buscar Supervisor ---
            SupervisorModel supervisor = _supervisorService.BuscarPorId(id);
            if (supervisor == null)
            {
                return StatusCode(404, "Supervisor não encontrado.");
            }

            // --- Verificação de Permissão ---
            if (usuarioLogado.Perfil == Perfil.Representante)
            {
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();
                if (supervisor.EmpresaId != empresaId.Value)
                {
                    return StatusCode(403, "Acesso negado.");
                }
            }

            // Retorna o novo formulário readonly
            return PartialView("_Deletar", supervisor);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deletar(SupervisorModel supervisor)
        {
            Console.WriteLine($"===== INÍCIO DO MÉTODO POST DELETAR (ID: {supervisor.Id}) =====");

            // --- 1. VERIFICAÇÃO DE SESSÃO E PERMISSÃO ---
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                return StatusCode(401, "Sessão expirada.");
            }

            SupervisorModel supervisorParaDeletar = _supervisorService.BuscarPorId(supervisor.Id);
            if (supervisorParaDeletar == null)
            {
                return StatusCode(404, "Supervisor não encontrado.");
            }

            if (usuarioLogado.Perfil == Perfil.Representante)
            {
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();
                if (supervisorParaDeletar.EmpresaId != empresaId.Value)
                {
                    return StatusCode(403, "Acesso negado.");
                }
            }

            // --- 2. CHAMADA AO SERVIÇO (AO INVÉS DO REPOSITÓRIO) ---
            try
            {
                // _supervisorRepository.Deletar(supervisor.Id); // <-- Linha antiga
                _supervisorService.DeletarSupervisor(supervisor.Id); // <-- NOVA LINHA

                Console.WriteLine("Supervisor deletado com SUCESSO!");
                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO DE BANCO DE DADOS: {ex.Message}");
                return StatusCode(500, $"Erro de banco de dados ao excluir: {ex.Message}");
            }
        }

        // MÉTODO DE VALIDAÇÃO [Remote]
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarCPFUnico(string cpf)
        {
            bool cpfJaExiste = await _supervisorService.VerificarCPFUnico(cpf);

            if (cpfJaExiste)
            {
                return Json($"O CPF {cpf} já está cadastrado.");
            }
            return Json(true);
        }

        public IActionResult Login()
        {
            return View("Login");
        }
    }
}
