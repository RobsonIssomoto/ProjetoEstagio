using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface IUsuarioRepository
    {
        UsuarioModel Cadastrar(UsuarioModel usuario);
        List<UsuarioModel> ListarTodos();
        UsuarioModel BuscarPorId(int id);
        UsuarioModel BuscarPorLogin(string login);
        UsuarioModel Atualizar(UsuarioModel usuario);

        Task<bool> VerificarEmailUnico(string email);

        bool Deletar(int id);
    }
}
