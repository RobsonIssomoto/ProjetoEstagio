using ProjetoEstagio.Models;
using ProjetoEstagio.Models.ViewModels;

namespace ProjetoEstagio.Services
{
    public interface IUsuarioService
    {
        UsuarioModel BuscarPorLogin(string login);

        // --- NOVO MÉTODO ---
        // Retorna uma string com a mensagem de sucesso ou lança exceção em caso de erro
        void RedefinirSenha(string login);
        void AlterarSenha(AlterarSenhaViewModel viewModel);
    }
}