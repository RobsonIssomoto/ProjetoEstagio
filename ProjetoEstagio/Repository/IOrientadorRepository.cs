// Repository/IOrientadorRepository.cs
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface IOrientadorRepository
    {
        OrientadorModel Cadastrar(OrientadorModel orientador);
        List<OrientadorModel> ListarTodos();
        OrientadorModel BuscarPorId(int id);
        OrientadorModel BuscarPorUsuarioId(int usuarioId);
        OrientadorModel Atualizar(OrientadorModel orientador);
        bool Deletar(int id);
    }
}