using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public class TermoCompromissoRepository : ITermoCompromissoRepository
    {
        private readonly ProjetoEstagioContext _context;

        public TermoCompromissoRepository(ProjetoEstagioContext context)
        {
            _context = context;
        }

        public TermoCompromissoModel Cadastrar(TermoCompromissoModel termo)
        {
            _context.TermosCompromisso.Add(termo);
            _context.SaveChanges();
            return termo;
        }

        public TermoCompromissoModel BuscarPorId(int id)
        {
            return _context.TermosCompromisso.FirstOrDefault(t => t.Id == id);
        }

        public TermoCompromissoModel BuscarCompletoPorId(int id)
        {
            // Carrega o Termo E os dados da Solicitação (Aluno e Empresa)
            return _context.TermosCompromisso
                .Include(t => t.SolicitacaoEstagio)         // Inclui a Solicitação
                    .ThenInclude(s => s.Estagiario)         // Da Solicitação, inclui o Estagiário
                .Include(t => t.SolicitacaoEstagio)         // Inclui a Solicitação de novo
                    .ThenInclude(s => s.Empresa)            // Da Solicitação, inclui a Empresa
                .FirstOrDefault(t => t.Id == id);           // Filtra pelo ID do Termo
        }
        public TermoCompromissoModel Atualizar(TermoCompromissoModel termo)
        {
            var termoDB = BuscarPorId(termo.Id);
            if (termoDB == null)
            {
                throw new Exception("Termo de Compromisso não encontrado.");
            }

            // Atualiza os campos preenchidos pela empresa
            termoDB.CargaHoraria = termo.CargaHoraria;
            termoDB.ValorBolsa = termo.ValorBolsa;
            termoDB.DataInicio = termo.DataInicio;
            termoDB.DataFim = termo.DataFim;
            termoDB.NumeroApolice = termo.NumeroApolice;
            termoDB.NomeSeguradora = termo.NomeSeguradora;
            // O OrientadorId será atualizado pelo Admin

            _context.TermosCompromisso.Update(termoDB);
            _context.SaveChanges();
            return termoDB;
        }
    }
}