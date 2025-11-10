// Repository/ISolicitacaoEstagioRepository.cs
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface ISolicitacaoEstagioRepository
    {
        SolicitacaoEstagioModel Cadastrar(SolicitacaoEstagioModel solicitacao);
        SolicitacaoEstagioModel BuscarPorId(int id);
        List<SolicitacaoEstagioModel> BuscarPorEstagiario(int estagiarioId);
        List<SolicitacaoEstagioModel> ListarPorEmpresaId(int empresaId);
        // (Adicione outros métodos conforme necessário, como Atualizar, Deletar, etc.)
        SolicitacaoEstagioModel BuscarPorToken(string token);
        SolicitacaoEstagioModel Atualizar(SolicitacaoEstagioModel solicitacao);
    }
}