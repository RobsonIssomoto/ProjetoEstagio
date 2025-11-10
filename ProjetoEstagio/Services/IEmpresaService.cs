using ProjetoEstagio.Models;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Repository;
using ProjetoEstagio.Data; // 2. Adicionado


namespace ProjetoEstagio.Services
{
    public interface IEmpresaService
    {
        EmpresaModel Cadastrar(EmpresaModel empresa);
        List<EmpresaModel> ListarTodos();
        EmpresaModel BuscarPorId(int id);
        EmpresaModel BuscarComSupervisores(int id);
        EmpresaModel BuscarPorUsuarioId(int usuarioId);
        EmpresaModel BuscarEmpresaPorUsuarioId(int usuarioId);
        List<SolicitacaoEstagioModel> ListarSolicitacoes(int empresaId);
        EmpresaModel Atualizar(EmpresaModel empresa);
        bool Deletar(int id);
        void RegistrarNovaEmpresa(EmpresaCadastroViewModel viewModel, string? token);
        SolicitacaoEstagioModel BuscarSolicitacaoPorId(int solicitacaoId);
        //TermoCompromissoModel AprovarSolicitacao(int solicitacaoId);
        void SalvarTermo(TermoPreenchimentoViewModel viewModel, int empresaIdLogada);
        void RejeitarTermo(TermoPreenchimentoViewModel viewModel, int empresaIdLogada);
        TermoCompromissoModel BuscarOuCriarTermoPorSolicitacao(int solicitacaoId, int empresaIdLogada);
        //TermoCompromissoModel BuscarTermoCompleto(int termoId);
        //void SalvarTermoPreenchido(TermoPreenchimentoViewModel viewModel);
        Task<bool> VerificarCNPJUnico(string cnpj);
    }
}
