// Repository/SolicitacaoEstagioRepository.cs
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public class SolicitacaoEstagioRepository : ISolicitacaoEstagioRepository
    {
        private readonly ProjetoEstagioContext _context;

        public SolicitacaoEstagioRepository(ProjetoEstagioContext context)
        {
            _context = context;
        }

        public SolicitacaoEstagioModel Cadastrar(SolicitacaoEstagioModel solicitacao)
        {
            _context.SolicitacoesEstagio.Add(solicitacao);
            _context.SaveChanges();
            return solicitacao;
        }

        public SolicitacaoEstagioModel BuscarPorId(int id)
        {
            return _context.SolicitacoesEstagio.FirstOrDefault(s => s.Id == id);
        }

        public List<SolicitacaoEstagioModel> BuscarPorEstagiario(int estagiarioId)
        {
            return _context.SolicitacoesEstagio
                           .Where(s => s.EstagiarioId == estagiarioId)
                           .ToList();
        }

        public List<SolicitacaoEstagioModel> ListarPorEmpresaId(int empresaId)
        {
            return _context.SolicitacoesEstagio
                           .Include(s => s.Estagiario) // Carrega os dados do estagiário
                           .Include(s => s.TermoCompromisso)
                           .Where(s => s.EmpresaId == empresaId)
                           .OrderByDescending(s => s.DataSubmissao)
                           .ToList();
        }

        // --- IMPLEMENTAÇÃO DO MÉTODO NOVO ---
        public SolicitacaoEstagioModel BuscarPorToken(string token)
        {
            // Busca a solicitação que corresponde ao token
            return _context.SolicitacoesEstagio.FirstOrDefault(s => s.Token == token);
        }

        // --- IMPLEMENTAÇÃO DO MÉTODO NOVO ---
        public SolicitacaoEstagioModel Atualizar(SolicitacaoEstagioModel solicitacao)
        {
            // Busca a versão do banco de dados
            var solicitacaoDB = BuscarPorId(solicitacao.Id);
            if (solicitacaoDB == null)
            {
                throw new System.Exception("Erro ao atualizar solicitação: ID não encontrado.");
            }

            // Atualiza os campos que mudaram
            solicitacaoDB.EmpresaId = solicitacao.EmpresaId;
            solicitacaoDB.Status = solicitacao.Status;
            solicitacaoDB.Token = solicitacao.Token; // Para anular o token

            _context.SolicitacoesEstagio.Update(solicitacaoDB);
            _context.SaveChanges();
            return solicitacaoDB;
        }
    }
}