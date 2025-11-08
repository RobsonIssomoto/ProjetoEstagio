using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface IEstagiarioRepository
    {
        EstagiarioModel Cadastrar(EstagiarioModel estagiario);
        List<EstagiarioModel> ListarTodos();
        EstagiarioModel BuscarPorId(int id);
        EstagiarioModel BuscarPorUsuarioId(int usuarioId);
        EstagiarioModel Editar(EstagiarioModel estagiario);
        EstagiarioModel Atualizar(EstagiarioModel estagiario);
        bool Deletar(int id);
    }
}
