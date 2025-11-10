using ProjetoEstagio.Models;

namespace ProjetoEstagio.Helper
{
    public interface ISessao
    {
        void CriarSessaoDoUsuario(UsuarioModel usuario);
        void RemoverSessaoDoUsuario();
        void SalvarEmpresaIdNaSessao(int empresaId);
        int? BuscarEmpresaIdDaSessao();
        void SalvarEstagiarioIdNaSessao(int estagiarioId);
        int? BuscarEstagiarioIdDaSessao();
        void SalvarNomeExibicao(string nome);
        string BuscarNomeExibicao();

        UsuarioModel BuscarSessaoDoUsuario();
    }
}
