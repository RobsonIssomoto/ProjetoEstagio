using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface IUsuarioRepository
    {
        UsuarioModel Cadastrar(UsuarioModel usuario);
        List<UsuarioModel> ListarTodos();
        UsuarioModel BuscarPorId(int id);
        UsuarioModel BuscarPorLogin(string login);
        UsuarioModel Editar(UsuarioModel usuario);
        UsuarioModel Atualizar(UsuarioModel usuario);
        bool Deletar(int id);
    }
}
