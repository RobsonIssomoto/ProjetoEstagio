using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Filters;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Repository;
using ProjetoEstagio.Services;

namespace ProjetoEstagio.Controllers
{

    public class UsuarioController : Controller
    {

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ISessao _sessao;
        public UsuarioController(IUsuarioRepository usuarioRepository, IUsuarioService usuarioService, ISessao sessao)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _sessao = sessao;

        }
        [Autorizacao(Perfil.Admin)]
        public IActionResult Index()
        {
            // Usa o método novo do repositório
            List<UsuarioModel> admins = _usuarioRepository.ListarAdmins();
            return View(admins);
        }

        public IActionResult Principal()
        {
            return View();
        }

        public IActionResult Cadastrar()
        {
            return View(new UsuarioCadastroViewModel()); // <-- MUDANÇA
        }

        [HttpPost]
        public IActionResult Cadastrar(UsuarioCadastroViewModel viewModel) // <-- MUDANÇA
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 1. Cria o UsuarioModel a partir do ViewModel
                    var usuario = new UsuarioModel
                    {
                        Login = viewModel.Email,
                        Email = viewModel.Email,
                        Perfil = viewModel.Perfil
                    };

                    // 2. CHAMA O SETSENHAHASH (A CORREÇÃO)
                    usuario.SetSenhaHash(viewModel.Senha); // 

                    // 3. Salva no repositório
                    _usuarioRepository.Cadastrar(usuario);

                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} no cadastro de usuário. Tente novamente";
                return RedirectToAction("Index");
            }

            // Se o ModelState for inválido, retorna o ViewModel para a View
            return View(viewModel);
        }

        public IActionResult DeletarConfirmar(int id)
        {
            UsuarioModel usuario = _usuarioRepository.BuscarPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        public IActionResult Deletar(int id)
        {
            try
            {
                bool deletar = _usuarioRepository.Deletar(id);

                if (deletar)
                {
                    TempData["MensagemSucesso"] = "Usuário excluído com sucesso";

                }
                else
                {
                    TempData["MensagemErro"] = $"Erro ao excluir usuário. Tente novamente";
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Devido erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Editar(int id)
        {
            UsuarioModel usuario = _usuarioRepository.BuscarPorId(id);
            return View(usuario);
        }

        [HttpPost]
        public IActionResult Alterar(UsuarioSemSenhaModel usuarioSemSenhaModel)
        {
            try
            {
                // O ModelState é validado contra o UsuarioSemSenhaModel, o que está correto.
                if (ModelState.IsValid)
                {
                    // 1. LER: Busque o usuário COMPLETO do banco de dados primeiro.
                    UsuarioModel usuarioDoBanco = _usuarioRepository.BuscarPorId(usuarioSemSenhaModel.Id);

                    if (usuarioDoBanco == null)
                    {
                        TempData["MensagemErro"] = "Erro ao atualizar: Usuário não encontrado.";
                        return RedirectToAction("Index");
                    }

                    // 2. MODIFICAR: Atualize apenas os dados que vieram do formulário.
                    //    As outras propriedades (Senha, DataCadastro) permanecem intactas.
                    usuarioDoBanco.Login = usuarioSemSenhaModel.Login;
                    usuarioDoBanco.Email = usuarioSemSenhaModel.Email;
                    usuarioDoBanco.Perfil = usuarioSemSenhaModel.Perfil;
                    usuarioDoBanco.DataAtualizacao = DateTime.Now; // Boa prática!

                    // 3. SALVAR: Envie o objeto completo e atualizado para o repositório.
                    _usuarioRepository.Atualizar(usuarioDoBanco); // Agora 'usuarioDoBanco' está completo.

                    TempData["MensagemSucesso"] = "Dados do usuário alterados com sucesso";
                    return RedirectToAction("Index");
                }

                // Se o ModelState for inválido, precisamos retornar para a View "Editar".
                // Mas a view "Editar" espera um UsuarioModel, não um UsuarioSemSenhaModel.
                // Isso pode causar um erro. O ideal é que a view "Editar" também use "UsuarioSemSenhaModel".
                // Por enquanto, vamos apenas retornar o modelo inválido.
                return View("Editar", usuarioSemSenhaModel);
            }
            catch (System.Exception erro)
            {
                // Para debugar, o "inner exception" é o mais importante.
                // Considere logar o 'erro.InnerException'
                TempData["MensagemErro"] = $"Erro ao salvar: {erro.Message}. Tente novamente.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlterarSenha(AlterarSenhaViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Chama o serviço centralizado
                    _usuarioService.AlterarSenha(viewModel);

                    return Json(new { sucesso = true, mensagem = "Senha alterada com sucesso!" });
                }

                // Se a validação falhar (ex: senhas não batem), retorna os erros
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(string.Join("<br>", erros));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Retorna erro 500 com a mensagem (ex: "Senha atual incorreta")
            }
        }

        [HttpGet]
        public IActionResult MeuPerfil()
        {
            // 1. Busca o usuário da sessão
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            // 2. Segurança: Só Admin entra aqui
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin)
            {
                return RedirectToAction("Index", "Login");
            }

            // 3. Busca os dados frescos do banco
            UsuarioModel admin = _usuarioRepository.BuscarPorId(usuarioLogado.Id);
            if (admin == null) return RedirectToAction("Index", "Login");

            // 4. Monta o ViewModel
            var viewModel = new AdminEditarViewModel
            {
                Id = admin.Id,
                Email = admin.Email
            };

            return View("EditarAdmin", viewModel);
        }

        [HttpGet]
        [Autorizacao(Perfil.Admin)]
        public IActionResult CadastrarAdmin()
        {
            // Retorna o formulário dentro do modal (Partial)
            return PartialView("_CadastrarAdmin", new AdminCadastroViewModel());
        }

        // 3. POST CADASTRAR: Agora retorna JSON para o AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Autorizacao(Perfil.Admin)]
        public IActionResult CadastrarAdmin(AdminCadastroViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _usuarioService.CadastrarAdmin(viewModel);

                    // Sucesso: Retorna JSON para o site.js fechar o modal e recarregar
                    return Json(new { sucesso = true });
                }
                catch (Exception ex)
                {
                    // Erro do servidor (ex: email duplicado)
                    return StatusCode(500, $"Erro ao cadastrar: {ex.Message}");
                }
            }

            // Erro de validação: Retorna os erros para exibir no modal
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlterarAdmin(AdminEditarViewModel viewModel)
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null || usuarioLogado.Perfil != Perfil.Admin) return RedirectToAction("Index", "Login");

            // Segurança: O Admin só pode editar a SI MESMO nesta tela
            if (usuarioLogado.Id != viewModel.Id)
            {
                return RedirectToAction("Index", "Restrito");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    UsuarioModel adminDB = _usuarioRepository.BuscarPorId(viewModel.Id);

                    // Atualiza Email e Login (mantendo sincronizados)
                    adminDB.Email = viewModel.Email;
                    adminDB.Login = viewModel.Email;
                    adminDB.DataAtualizacao = DateTime.Now;

                    _usuarioRepository.Atualizar(adminDB);

                    // Atualiza a sessão com o novo nome/email
                    _sessao.SalvarNomeExibicao(adminDB.Login);

                    TempData["MensagemSucesso"] = "Perfil atualizado com sucesso!";
                    return RedirectToAction("Pendencias", "Orientador"); // Volta para o painel principal
                }
                catch (Exception ex)
                {
                    TempData["MensagemErro"] = $"Erro ao atualizar: {ex.Message}";
                }
            }

            return View("EditarAdmin", viewModel);
        }

        // Método [Remote] para validar Email
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarEmailUnico(string email)
        {
            // Chama o repositório correto, que já está injetado.
            bool emailJaExiste = await _usuarioRepository.VerificarEmailUnico(email);

            if (emailJaExiste)
            {
                return Json($"O e-mail {email} já está em uso.");
            }

            return Json(true);
        }
    }
}
