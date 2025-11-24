using ProjetoEstagio.Models;
using ProjetoEstagio.Models.ViewModels;

namespace ProjetoEstagio.Services
{
    public interface IUsuarioService
    {
        UsuarioModel BuscarPorLogin(string login);
        void CadastrarAdmin(AdminCadastroViewModel viewModel);
        void RedefinirSenha(string login);
        void AlterarSenha(AlterarSenhaViewModel viewModel);

    }
}