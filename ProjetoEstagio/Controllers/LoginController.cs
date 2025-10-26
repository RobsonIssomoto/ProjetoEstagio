using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISessao _sessao;
        public LoginController(IUsuarioRepository usuarioRepository, ISessao sessao)
        {
            _usuarioRepository = usuarioRepository;
            _sessao = sessao;
        }
        public IActionResult Index()
        {
            if (_sessao.BuscarSessaoDoUsuario() != null) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = _usuarioRepository.BuscarPorLogin(loginModel.Login);

                    // Checa se o usuário existe E se a senha é válida (com o BCrypt)
                    if (usuario != null && usuario.SenhaValida(loginModel.Senha))
                    {
                        // SUCESSO: Cria a sessão e redireciona
                        _sessao.CriarSessaoDoUsuario(usuario);
                        return RedirectToAction("Principal", "Estagiario");
                    }

                    // FALHA DE AUTENTICAÇÃO: Usuário nulo OU senha errada
                    TempData["MensagemErro"] = $"Usuário e/ou Senha inválido(s)! Tente novamente.";
                }
                else
                {
                    // FALHA DE VALIDAÇÃO: Campos vazios, etc.
                    TempData["MensagemErro"] = "Por favor, preencha todos os campos.";
                }

                // --- PONTO COMUM DE FALHA (Autenticação ou Validação) ---

                // Limpa o modelo
                loginModel.Login = string.Empty;
                loginModel.Senha = string.Empty;

                // Limpa o ModelState para forçar o asp-for a usar o modelo limpo
                ModelState.Clear();

                return View("Index", loginModel);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao tentar realizar Login. Tente novamente: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Logout()
        {
            _sessao.RemoverSessaoDoUsuario();
            return RedirectToAction("Index", "Login");
        }
    }
}
