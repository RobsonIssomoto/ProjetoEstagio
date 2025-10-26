using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;
using ProjetoEstagio.Repository;

namespace ProjetoEstagio.Controllers
{
    public class EmpresaController : Controller
    {

        //private readonly ProjetoEstagioContext _context;

        //public EmpresaController(ProjetoEstagioContext context)
        //{
        //    _context = context;
        //}
        public EmpresaController(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        private readonly IEmpresaRepository _empresaRepository;


        public IActionResult Index()
        {
            List<EmpresaModel> empresas = _empresaRepository.ListarTodos();
            return View(empresas);
        }



        //public async Task<IActionResult> Index()
        //{
        //    var empresas = await _context.Empresas.ToListAsync();
        //    return View(empresas);
        //}

        public IActionResult Cadastrar()
        {
            return View();
        }

        public IActionResult Principal()
        {
            List<EmpresaModel> empresas = _empresaRepository.ListarTodos();
            return View(empresas);
        }

        [HttpPost]
        public IActionResult Cadastrar(EmpresaModel empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _empresaRepository.Cadastrar(empresa);
                    TempData["MensagemSucesso"] = "Empresa cadastrada com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro {erro.Message} no cadastro da empresa. Tente novamente";
                return RedirectToAction("Index");
            }
            return View(empresa);
        }




        [HttpPost]
        //public async Task<IActionResult> Cadastrar([Bind("Id, Nome, CNPJ, Email")] EmpresaModel empresa)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Empresas.Add(empresa);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(empresa);
        //}

        // GET: /Empresa/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            EmpresaModel empresa = _empresaRepository.BuscarPorId(id);

            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // POST: /Empresa/Editar
        [HttpPost]
        public IActionResult Editar(EmpresaModel empresa)
        {
            if (ModelState.IsValid)
            {
                _empresaRepository.Editar(empresa);
                return RedirectToAction("Index");
            }

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



        //[HttpGet] 
        //public IActionResult Index(string cnpj = null)
        //{
        //    Empresa model = new Empresa();

        //    if (!string.IsNullOrEmpty(cnpj))
        //    {
        //        model = _context.empresas.FirstOrDefault(e => e.CNPJ == cnpj);
        //        if (model == null)
        //            return NotFound();
        //    }

        //    //ViewBag.Empresas = _context.empresas.ToList();
        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult Save(Empresa empresa, string action)
        //{
        //    if (action == "cadastrar" && ModelState.IsValid)
        //    {
        //        _context.empresas.Add(empresa);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    else if (action == "atualizar" && ModelState.IsValid)
        //    {
        //        _context.empresas.Update(empresa);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }


        //    ViewBag.Empresas = _context.empresas.ToList();
        //    return View("Index");
        //}


        //[HttpPost]

        //public IActionResult DeleteConfirmedPorCNPJ(string cnpj)
        //{
        //    if (string.IsNullOrWhiteSpace(cnpj))
        //        return BadRequest("CNPJ inválido.");

        //    var empresa = _context.empresas.FirstOrDefault(e => e.CNPJ == cnpj);
        //    if (empresa != null)
        //    {
        //        _context.empresas.Remove(empresa);
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}

        //public IActionResult Cadastro()
        //{
        //    return View("Cadastro");
        //}

        public IActionResult Login()
        {
            return View("Login");
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

        // ... (O resto dos seus métodos: Deletar, Login, etc.) ...
    }

    //public IActionResult Principal()
    //{
    //    return View("Principal");
    //}

    //public IActionResult CadastroEstagio()
    //{
    //    return View("CadastroEstagio");
    //}

    //private readonly ISupervisorRepository _supervisorRepository;

    //public EmpresaController(ISupervisorRepository supervisorRepository)
    //{
    //    var supervisores = _supervisorRepository.ListarTodos();
    //    ViewBag.SupervisorList = new SelectList(supervisores, "Id", "Nome");
    //    return View();
    //}
}

