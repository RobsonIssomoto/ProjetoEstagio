// Services/IOrientadorService.cs
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.ViewModels;

namespace ProjetoEstagio.Services
{
    public interface IOrientadorService
    {
        void RegistrarNovoOrientador(OrientadorCadastroViewModel viewModel);
        void AtualizarOrientador(OrientadorModel orientador);
        void DeletarOrientador(int orientadorId);
        OrientadorModel BuscarPorId(int id);
        List<OrientadorModel> ListarTodos();
        Task<bool> VerificarCPFUnico(string cpf);
    }
}