using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// 1. Usings adicionados
using ProjetoEstagio.Data;
using ProjetoEstagio.Filters;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels; // <-- Adicionado
using ProjetoEstagio.Repository;


namespace ProjetoEstagio.Controllers
{
    public class EstagiarioController : Controller
    {
        // 3. Dependências Adicionadas
        private readonly IEstagiarioRepository _estagiarioRepository;
        private readonly IUsuarioRepository _usuarioRepository; // <-- Adicionado
        private readonly ProjetoEstagioContext _context; // <-- Adicionado
        private readonly ISessao _sessao;

        public EstagiarioController(
            IEstagiarioRepository estagiarioRepository,
            IUsuarioRepository usuarioRepository, // <-- Adicionado
            ProjetoEstagioContext context,
            ISessao sessao) // <-- Adicionado
        {
            _estagiarioRepository = estagiarioRepository;
            _usuarioRepository = usuarioRepository; // <-- Adicionado
            _context = context; // <-- Adicionado
            _sessao = sessao;
        }

        public IActionResult Index()
        {
            List<EstagiarioModel> estagiario = _estagiarioRepository.ListarTodos();
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
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        // 1. Criar o Objeto Usuario
                        var usuario = new UsuarioModel();
                        usuario.Login = viewModel.Email; // Login é o email
                        usuario.Email = viewModel.Email;
                        usuario.Perfil = Perfil.Estagiario;
                        usuario.SetSenhaHash(viewModel.Senha); // Seta o HASH

                        _usuarioRepository.Cadastrar(usuario); // Salva o usuário

                        // 2. Criar o Objeto Estagiario
                        var estagiario = new EstagiarioModel
                        {
                            Nome = viewModel.Nome,
                            CPF = viewModel.CPF,
                            Telefone = viewModel.Telefone,
                            Email = viewModel.Email,
                            DataCadastro = DateTime.Now,
                            UsuarioId = usuario.Id // <-- O VÍNCULO!
                        };

                        _estagiarioRepository.Cadastrar(estagiario); // Salva o perfil

                        // 3. Se tudo deu certo, salva as mudanças no banco
                        transaction.Commit();

                        TempData["MensagemSucesso"] = "Cadastro realizado! Faça o login.";
                        return RedirectToAction("Index", "Login"); // Redireciona para o Login
                    }
                }
                catch (Exception erro)
                {
                    transaction.Rollback(); // Desfaz tudo
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
            EstagiarioModel estagiario = _estagiarioRepository.BuscarPorId(id);
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
                    _estagiarioRepository.Atualizar(estagiario);
                    TempData["MensagemSucesso"] = "Dados alterados com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} na alteração. Tente novamente";
                return RedirectToAction("Index");
            }
            return View("Editar", estagiario);
        }

        public IActionResult DeletarConfirmar(int id)
        {
            EstagiarioModel estagiario = _estagiarioRepository.BuscarPorId(id);
            if (estagiario == null) return NotFound();
            return View(estagiario);
        }

        public IActionResult Deletar(int id)
        {
            try
            {
                bool deletar = _estagiarioRepository.Deletar(id);
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
        /// Método para validação remota do CPF.
        /// </summary>
        /// <param name="cpf">O CPF vindo do formulário</param>
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarCPFUnico(string cpf)
        {
            // Opcional, mas recomendado: Limpar a formatação do CPF (pontos e traços)
            // var cpfLimpo = cpf.Replace(".", "").Replace("-", "");

            // Verifica se já existe um ESTAGIÁRIO com este CPF
            var cpfJaExiste = await _context.Estagiarios
                                    .AnyAsync(e => e.CPF == cpf); // ou e.CPF == cpfLimpo

            if (cpfJaExiste)
            {
                return Json($"O CPF {cpf} já está cadastrado.");
            }

            return Json(true);
        }

        public IActionResult Login() => View("Login");

        public IActionResult Processo()
        {
            try
            {
                // 1. Busca o ID DO ESTAGIÁRIO salvo na sessão
                // (Assumindo que sua classe _sessao tenha o método de leitura)
                int? estagiarioId = _sessao.BuscarEstagiarioIdDaSessao();

                if (estagiarioId == null)
                {
                    TempData["MensagemErro"] = "Sua sessão expirou.";
                    return RedirectToAction("Index", "Login");
                }

                // 2. Busca o estagiário COMPLETO usando o repositório
                //    Este método BuscarPorId DEVE buscar na tabela Estagiarios
                EstagiarioModel estagiario = _estagiarioRepository.BuscarPorId(estagiarioId.Value);

                if (estagiario == null)
                {
                    TempData["MensagemErro"] = "Erro: Perfil de estagiário não encontrado.";
                    return RedirectToAction("Index", "Login");
                }

                // 3. Envia o modelo PREENCHIDO para a View
                return View(estagiario);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao carregar página: {erro.Message}";
                return RedirectToAction("Index", "Login");
            }
        }
    }
}