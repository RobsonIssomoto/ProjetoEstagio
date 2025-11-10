using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface ISupervisorRepository
    {
        SupervisorModel Cadastrar(SupervisorModel supervisor);
        List<SupervisorModel> ListarTodos();
        List<SupervisorModel> ListarPorEmpresa(int empresaId);
        SupervisorModel BuscarPorId(int id);
        SupervisorModel BuscarPorUsuarioId(int usuarioId);
        SupervisorModel Atualizar(SupervisorModel supervisor);
        bool Deletar(int id);
    }
}
