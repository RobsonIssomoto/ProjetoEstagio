using ProjetoEstagio.Models;
using ProjetoEstagio.Models.ViewModels;

namespace ProjetoEstagio.Services
{

    // Em IEstagiarioService.cs
    public interface IEstagiarioService
    {
        // O que você já tem
        void RegistrarNovoEstagiario(EstagiarioCadastroViewModel viewModel);
        Task<bool> VerificarCPFUnico(string cpf);

        // --- ADICIONE ESTES ---
        List<EstagiarioModel> ListarTodos();
        EstagiarioModel BuscarPorId(int id);
        EstagiarioModel Atualizar(EstagiarioModel estagiario);
        bool Deletar(int id);
        EstagiarioModel BuscarPorUsuarioId(int usuarioId); // Você tinha isso no repositório
        void CriarSolicitacao(SolicitacaoCadastroViewModel viewModel);
    }
}
