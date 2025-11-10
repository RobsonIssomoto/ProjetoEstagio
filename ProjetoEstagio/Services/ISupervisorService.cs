using ProjetoEstagio.Models;
using ProjetoEstagio.Models.ViewModels;

namespace ProjetoEstagio.Services
{
    public interface ISupervisorService
    {
        void RegistrarNovoSupervisor(SupervisorCadastroViewModel viewModel, int empresaId);
        void AtualizarSupervisor(SupervisorModel supervisor);
        void DeletarSupervisor(int supervisorId);
        SupervisorModel BuscarPorId(int id);
        List<SupervisorModel> ListarTodos();
        List<SupervisorModel> ListarPorEmpresa(int empresaId);

        // --- ADICIONE O MÉTODO DE VALIDAÇÃO ---
        Task<bool> VerificarCPFUnico(string cpf);
    }
}