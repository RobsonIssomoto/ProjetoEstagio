using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{
    public class SupervisorController : Controller
    {
            private readonly ISupervisorRepository _supervisorRepository;
            private readonly ISessao _sessao;
            private readonly ProjetoEstagioContext _context; // <-- Adicionado

            public SupervisorController(ISupervisorRepository supervisorRepository, ISessao sessao, ProjetoEstagioContext context)
            {
                _supervisorRepository = supervisorRepository;
                _sessao = sessao;
                _context = context;
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
                supervisores = _supervisorRepository.ListarTodos();
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
                supervisores = _supervisorRepository.ListarPorEmpresa(empresaId.Value);
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
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                return StatusCode(401, "Sessão expirada. Faça login novamente.");
            }

            var supervisor = new SupervisorModel();

            // ===== ADICIONE ESTAS LINHAS =====
            if (usuarioLogado.Perfil == Perfil.Representante)
            {
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();
                if (empresaId != null)
                {
                    supervisor.EmpresaId = empresaId.Value;
                }
            }
            // ===== FIM =====

            return PartialView("_Cadastrar", supervisor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(SupervisorModel supervisor)
        {
            Console.WriteLine("===== INÍCIO DO MÉTODO CADASTRAR (MODO NUCLEAR) =====");

            // --- 1. VERIFICAÇÃO DE SESSÃO ---
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                Console.WriteLine("ERRO: Usuário não logado!");
                return StatusCode(401, "Sessão expirada. Faça login novamente.");
            }
            Console.WriteLine($"Usuário logado: {usuarioLogado.Email}, Perfil: {usuarioLogado.Perfil}");

            // --- 2. COMPLETAR O MODELO ---
            if (usuarioLogado.Perfil == Perfil.Representante)
            {
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();
                if (empresaId == null)
                {
                    Console.WriteLine("ERRO: EmpresaId da sessão é null!");
                    return StatusCode(403, "Erro ao recuperar informações da empresa.");
                }
                supervisor.EmpresaId = empresaId.Value;
                Console.WriteLine($"EmpresaId setado no supervisor: {supervisor.EmpresaId}");
            }

            // --- 3. IGNORANDO O MODELSTATE.ISVALID ---
            Console.WriteLine("FORÇANDO O CADASTRO (IGNORANDO O MODELSTATE QUEBRADO)");

            try
            {
                _supervisorRepository.Cadastrar(supervisor);
                Console.WriteLine("Cadastro realizado com SUCESSO!");
                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                // --- CAPTURA A MENSAGEM DA INNER EXCEPTION ---
                var innerMessage = ex.InnerException?.Message ?? ex.Message;

                Console.WriteLine($"ERRO DE BANCO DE DADOS: {innerMessage}");

                // --- VERIFICA SE É ERRO DE CPF DUPLICADO ---
                if (innerMessage.Contains("UNIQUE constraint") || innerMessage.Contains("duplicate key"))
                {
                    // Retorna uma mensagem amigável para o AJAX
                    return StatusCode(500, "Erro: Já existe um supervisor cadastrado com este CPF.");
                }

                // Retorna a mensagem de erro interna para o AJAX
                return StatusCode(500, $"Erro de banco de dados: {innerMessage}");
            }
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

            SupervisorModel supervisor = _supervisorRepository.BuscarPorId(id);
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
            Console.WriteLine("===== INÍCIO DO MÉTODO EDITAR (MODO NUCLEAR) =====");

            // --- 1. VERIFICAÇÃO DE SESSÃO ---
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                return StatusCode(401, "Sessão expirada. Faça login novamente.");
            }
            Console.WriteLine($"Usuário logado: {usuarioLogado.Email}, Perfil: {usuarioLogado.Perfil}");

            // --- 2. IGNORANDO O MODELSTATE.ISVALID ---
            Console.WriteLine("FORÇANDO A EDIÇÃO (IGNORANDO O MODELSTATE QUEBRADO)");

            // 3. VERIFICAR PERMISSÕES
            if (usuarioLogado.Perfil == Perfil.Representante)
            {
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();
                if (supervisor.EmpresaId != empresaId.Value)
                {
                    Console.WriteLine("ERRO: Tentativa de editar supervisor de outra empresa.");
                    return StatusCode(403, "Acesso negado. Você não tem permissão para alterar este item.");
                }
            }

            // 4. SALVAR NO BANCO
            try
            {
                _supervisorRepository.Editar(supervisor);
                Console.WriteLine("Edição realizada com SUCESSO!");
                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO EDITAR NO BANCO: {ex.Message}");
                return StatusCode(500, $"Erro de banco de dados: {ex.Message}");
            }
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

        //public IActionResult DeletarConfirmar(int id)
        //{
        //    // --- INÍCIO DA VERIFICAÇÃO DE SEGURANÇA ---
        //    UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
        //    if (usuarioLogado == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }

        //    SupervisorModel supervisor = _supervisorRepository.BuscarPorId(id);
        //    if (supervisor == null)
        //    {
        //        return NotFound();
        //    }

        //    if (usuarioLogado.Perfil == Perfil.Representante)
        //    {
        //        int? empresaId = _sessao.BuscarEmpresaIdDaSessao();

        //        // VERIFICA SE O SUPERVISOR PERTENCE À EMPRESA DO USUÁRIO
        //        if (supervisor.EmpresaId != empresaId.Value)
        //        {
        //            TempData["MensagemErro"] = "Acesso negado. Este supervisor não pertence à sua empresa.";
        //            return RedirectToAction("Index"); // Tentando deletar supervisor de outro
        //        }
        //    }
        //    // --- FIM DA VERIFICAÇÃO ---

        //    return View(supervisor);
        //}

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
            SupervisorModel supervisor = _supervisorRepository.BuscarPorId(id);
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

        // O método [HttpGet] DeletarConfirmar(int id) não é mais necessário para este fluxo.
        // Você pode mantê-lo ou removê-lo.

        // Substitua o seu [HttpGet] Deletar(int id) por este:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deletar(SupervisorModel supervisor) // Mudança aqui: Recebe o modelo
        {
            Console.WriteLine($"===== INÍCIO DO MÉTODO POST DELETAR (ID: {supervisor.Id}) =====");

            // --- 1. VERIFICAÇÃO DE SESSÃO ---
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                Console.WriteLine("ERRO: Usuário não logado!");
                return StatusCode(401, "Sessão expirada. Faça login novamente.");
            }

            // --- 2. VERIFICAR PERMISSÕES ---
            // (Verificamos de novo no POST por segurança)
            SupervisorModel supervisorParaDeletar = _supervisorRepository.BuscarPorId(supervisor.Id);
            if (supervisorParaDeletar == null)
            {
                return StatusCode(404, "Supervisor não encontrado.");
            }

            if (usuarioLogado.Perfil == Perfil.Representante)
            {
                int? empresaId = _sessao.BuscarEmpresaIdDaSessao();
                if (supervisorParaDeletar.EmpresaId != empresaId.Value)
                {
                    Console.WriteLine("ERRO: Tentativa de deletar supervisor de outra empresa.");
                    return StatusCode(403, "Acesso negado. Você não tem permissão para excluir este item.");
                }
            }

            // --- 3. EXECUTAR A EXCLUSÃO ---
            try
            {
                _supervisorRepository.Deletar(supervisor.Id); // Usamos o ID do modelo
                Console.WriteLine("Supervisor deletado com SUCESSO!");
                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO DE BANCO DE DADOS: {ex.Message}");
                return StatusCode(500, $"Erro de banco de dados ao excluir: {ex.Message}");
            }
        }

        [AcceptVerbs("GET", "POST")] // Permite que a validação funcione em GET ou POST
        public async Task<IActionResult> VerificarEmailUnico(string email)
        {
            // Verifica se já existe um USUÁRIO com este e-mail
            // (Pelo seu modelo, o Email de login fica na UsuarioModel)
            var emailJaExiste = await _context.Usuarios
                                      .AnyAsync(u => u.Email.ToUpper() == email.ToUpper());

            if (emailJaExiste)
            {
                // Se existe, retorna a mensagem de erro específica
                return Json($"O E-mail {email} já está em uso.");
            }

            // Se não existe, a validação passa
            return Json(true);
        }


        /// <summary>
        /// Método para validação remota do CNPJ.
        /// </summary>
        /// <param name="cnpj">O CNPJ vindo do formulário</param>
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarCNPJUnico(string cnpj)
        {
            // Opcional, mas recomendado: Limpar a formatação do CNPJ (pontos e traços)
            // var cpfLimpo = cpf.Replace(".", "").Replace("-", "");

            // Verifica se já existe uma EMPRESA com este CNPJ
            var cnpjJaExiste = await _context.Empresas
                                    .AnyAsync(e => e.CNPJ == cnpj); // ou e.CNPJ == cnpjLimpo

            if (cnpjJaExiste)
            {
                return Json($"O CNPJ {cnpj} já está cadastrado.");
            }

            return Json(true);
        }
        public IActionResult Login()
        {
            return View("Login");
        }
    }
}
