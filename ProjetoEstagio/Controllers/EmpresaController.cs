using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Data; // 1. Adicionado
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums; // 2. Adicionado
using ProjetoEstagio.Models.ViewModels; // 3. Adicionado
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{
    public class EmpresaController : Controller
    {
        // 4. Dependências Injetadas
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ProjetoEstagioContext _context;

        public EmpresaController(
            IEmpresaRepository empresaRepository,
            IUsuarioRepository usuarioRepository, // Adicionado
            ProjetoEstagioContext context) // Adicionado
        {
            _empresaRepository = empresaRepository;
            _usuarioRepository = usuarioRepository;
            _context = context;
        }

        // --- LÓGICA DE CADASTRO PÚBLICO (Nova) ---

        // GET: /Empresa/Cadastrar
        public IActionResult Cadastrar()
        {
            // (Substitui seu 'Cadastrar' e 'Cadastrar1' antigos)
            return View(new EmpresaCadastroViewModel());
        }

        [HttpPost]
        public IActionResult Cadastrar(EmpresaCadastroViewModel viewModel)
        {
            // (Substitui seu [HttpPost] Cadastrar antigo)
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        // 1. Criar o Usuario
                        var usuario = new UsuarioModel();
                        usuario.Login = viewModel.Email;
                        usuario.Email = viewModel.Email;
                        // --- AQUI ESTÁ A MÁGICA ---
                        usuario.Perfil = Perfil.Representante; // Definido automaticamente
                                                               // -------------------------
                        usuario.SetSenhaHash(viewModel.Senha);

                        _usuarioRepository.Cadastrar(usuario);

                        // 2. Criar a Empresa
                        var empresa = new EmpresaModel
                        {
                            RazaoSocial = viewModel.RazaoSocial,
                            CNPJ = viewModel.CNPJ,
                            Nome = viewModel.Nome,
                            Telefone = viewModel.Telefone,
                            Email = viewModel.Email,
                            DataCadastro = DateTime.Now,
                            UsuarioId = usuario.Id // <-- O VÍNCULO!
                        };

                        _empresaRepository.Cadastrar(empresa);

                        // 3. Salva tudo
                        transaction.Commit();

                        TempData["MensagemSucesso"] = "Empresa cadastrada! Faça o login.";
                        return RedirectToAction("Index", "Login");
                    }
                }
                catch (System.Exception erro)
                {
                    transaction.Rollback();
                    TempData["MensagemErro"] = $"Erro ao cadastrar: {erro.Message}";
                }
            }
            return View(viewModel);
        }

        // --- MÉTODOS DE ADMIN (Seus métodos antigos) ---

        public IActionResult Index()
        {
            List<EmpresaModel> empresas = _empresaRepository.ListarTodos();
            return View(empresas);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            EmpresaModel empresa = _empresaRepository.BuscarPorId(id);
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
                    _empresaRepository.Atualizar(empresa);
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
            EmpresaModel empresa = _empresaRepository.BuscarPorId(id);

            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }


        public IActionResult Deletar(int id)
        {
            try
            {
                bool deletar = _empresaRepository.Deletar(id);

                if (deletar)
                {
                    TempData["MensagemSucesso"] = "Empresa excluída com sucesso";

                }
                else
                {
                    TempData["MensagemErro"] = $"Erro ao excluir empresa. Tente novamente";
                }
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
            if (id == null)
            {
                return NotFound();
            }

            // Usamos o NOVO método do repositório
            EmpresaModel empresa = _empresaRepository.BuscarComSupervisores(id);

            if (empresa == null)
            {
                return NotFound("Empresa não encontrada.");
            }

            // Envia o objeto 'empresa' (que agora contém a lista
            // de supervisores) para a View.
            return View(empresa);
        }

        public IActionResult Login() => View("Login");

        public IActionResult Principal()
        {
            return View();
        }
    }
}
