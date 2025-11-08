using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data; // Precisa do seu Context
using ProjetoEstagio.Models; // Precisa do EmpresaModel
using ProjetoEstagio.Repository;
using System.Linq; // Para o .Where() e .Select()
using System.Threading.Tasks; // Para o async/await

namespace ProjetoEstagio.Controllers
{
    [Route("api/empresa")] // Define a rota base da API
    [ApiController]
    public class EmpresaApiController : ControllerBase
    {


        // Injete seu DbContext (ou IEmpresaRepository, se tiver)
        private readonly ProjetoEstagioContext _context;
        private readonly IEmpresaRepository _empresaRepository;

        public EmpresaApiController(ProjetoEstagioContext context, IEmpresaRepository empresaRepository)
        {
            _context = context;
            _empresaRepository = empresaRepository;
        }

        // DTO simples para não expor seu Model inteiro na API
        public class EmpresaBuscaDTO
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string CNPJ { get; set; }
        }

        [HttpGet("buscar")] // Rota completa: /api/empresa/buscar?termo=...
        public async Task<IActionResult> Buscar([FromQuery] string termo)
        {
            if (string.IsNullOrEmpty(termo) || termo.Length < 3)
            {
                return Ok(new List<EmpresaBuscaDTO>());
            }

            var termoUpper = termo.ToUpper();

            // --- A MELHORIA ESTÁ AQUI ---
            var empresas = await _context.Empresas
                .Where(e =>
                    // Busca no Nome (Fantasia)
                    e.Nome.ToUpper().Contains(termoUpper) ||

                    // << ADICIONE ESTA LINHA >>
                    // Busca na Razão Social (onde está o "LTDA")
                    e.RazaoSocial.ToUpper().Contains(termoUpper) ||

                    // Busca no CNPJ
                    e.CNPJ.Contains(termo)
                )
                .Select(e => new EmpresaBuscaDTO // Seleciona só os dados necessários
                {
                    Id = e.Id,
                    Nome = e.Nome, // Mantenha o Nome Fantasia como principal
                    CNPJ = e.CNPJ
                })
                .Take(10) // Limita a 10 resultados
                .ToListAsync();

            return Ok(empresas);

        }
    }
}