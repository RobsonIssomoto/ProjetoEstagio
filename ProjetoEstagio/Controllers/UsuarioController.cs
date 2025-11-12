using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Filters;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{

    public class UsuarioController : Controller
    {

        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public IActionResult Index()
        {
            List<UsuarioModel> usuarios = _usuarioRepository.ListarTodos();
            return View(usuarios);
        }

        // --- MÉTODO GET ATUALIZADO ---
        // (Ele agora usa o ViewModel)
        public IActionResult Cadastrar()
        {
            return View(new UsuarioCadastroViewModel()); // <-- MUDANÇA
        }

        // --- MÉTODO POST ATUALIZADO ---
        // (Ele recebe o ViewModel e CRIPTOGRAFA a senha)
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
