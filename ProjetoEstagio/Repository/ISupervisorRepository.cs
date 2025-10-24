using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface ISupervisorRepository
    {
        SupervisorModel Cadastrar(SupervisorModel supervisor);
        List<SupervisorModel> ListarTodos();
        SupervisorModel BuscarPorId(int id);
        SupervisorModel Editar(SupervisorModel supervisor);
        SupervisorModel Atualizar(SupervisorModel supervisor);
        bool Deletar(int id);
    }
}
