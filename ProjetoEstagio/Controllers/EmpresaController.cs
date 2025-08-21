using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly AppDbContext _context;

        public EmpresaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string cnpj = null)
        {
            Empresa model = new Empresa();

            if (!string.IsNullOrEmpty(cnpj))
            {
                model = _context.empresas.FirstOrDefault(e => e.CNPJ == cnpj);
                if (model == null)
                    return NotFound();
            }

            //ViewBag.Empresas = _context.empresas.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Empresa empresa, string action)
        {
            if (action == "cadastrar" && ModelState.IsValid)
            {
                _context.empresas.Add(empresa);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else if (action == "atualizar" && ModelState.IsValid)
            {
                _context.empresas.Update(empresa);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            

            ViewBag.Empresas = _context.empresas.ToList();
            return View("Index");
        }

        
        [HttpPost]
        
        public IActionResult DeleteConfirmedPorCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return BadRequest("CNPJ inválido.");

            var empresa = _context.empresas.FirstOrDefault(e => e.CNPJ == cnpj);
            if (empresa != null)
            {
                _context.empresas.Remove(empresa);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
