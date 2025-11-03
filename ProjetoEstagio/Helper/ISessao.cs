using ProjetoEstagio.Models;

namespace ProjetoEstagio.Helper
{
    public interface ISessao
    {
        void CriarSessaoDoUsuario(UsuarioModel usuario);
        void RemoverSessaoDoUsuario();
        void SalvarEmpresaIdNaSessao(int empresaId);
        int? BuscarEmpresaIdDaSessao();

        UsuarioModel BuscarSessaoDoUsuario();
    }
}
