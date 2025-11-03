using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmpresaRepository _empresaRepository; // 2. ADICIONE O REPO DE EMPRESA
        private readonly ISessao _sessao;
        public LoginController(IUsuarioRepository usuarioRepository, IEmpresaRepository empresaRepository, ISessao sessao)
        {
            _usuarioRepository = usuarioRepository;
            _empresaRepository = empresaRepository;
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

                    if (usuario != null && usuario.SenhaValida(loginModel.Senha))
                    {
                        // SUCESSO: Cria a sessão principal do usuário
                        _sessao.CriarSessaoDoUsuario(usuario);

                        // --- 5. LÓGICA DE FILTRAGEM DE PERFIL ---
                        // (Assumindo que seu enum se chama Perfil.Representante)
                        if (usuario.Perfil == Perfil.Representante)
                        {
                            // Busca a empresa vinculada a este usuário (usando o Passo 1)
                            EmpresaModel empresa = _empresaRepository.BuscarPorUsuarioId(usuario.Id);

                            if (empresa != null)
                            {
                                // Salva o ID da Empresa na sessão (usando o Passo 2)
                                _sessao.SalvarEmpresaIdNaSessao(empresa.Id);
                               
             }
                            else
                            {
                                // ERRO GRAVE: O usuário é 'Empresa' mas não achou vínculo
                                TempData["MensagemErro"] = "Erro de integridade de dados: Usuário de empresa não localizado. Contate o suporte.";
                                _sessao.RemoverSessaoDoUsuario(); // Desloga o usuário
                                return View("Index", loginModel);
                            }
                        }
                        // --- FIM DA LÓGICA DE FILTRAGEM ---

                        // Se for Admin ou Estagiário, ou se for Empresa e passou na checagem,
                        // ele redireciona normalmente.
                        return RedirectToAction("Index", "Usuario"); // (ou "Home")
                    }

                    // Seu código de "Usuário e/ou Senha inválido(s)!" não muda
                    TempData["MensagemErro"] = $"Usuário e/ou Senha inválido(s)! Tente novamente.";
                }
                else
                {
                    // Seu código de "Por favor, preencha todos os campos." não muda
                    TempData["MensagemErro"] = "Por favor, preencha todos os campos.";
                }

                // ... Seu código de limpar modelo e retornar View ...
                loginModel.Login = string.Empty;
                loginModel.Senha = string.Empty;
                ModelState.Clear();
                return View("Index", loginModel);
            }
            catch (Exception erro)
            {
                // Este bloco não muda
                TempData["MensagemErro"] = $"Erro ao tentar realizar Login. Tente novamente: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
        public IActionResult PreLogin()
        {
            return View();
        }

        public IActionResult Logout()
        {
            _sessao.RemoverSessaoDoUsuario();
            return RedirectToAction("Index", "Login");
        }
    }
}
