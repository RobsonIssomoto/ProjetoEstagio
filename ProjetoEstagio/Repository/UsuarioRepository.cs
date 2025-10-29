using ProjetoEstagio.Data;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ProjetoEstagioContext _projetoEstagioContext;

        public UsuarioRepository(ProjetoEstagioContext projetoEstagioContext)
        {
            _projetoEstagioContext = projetoEstagioContext;
        }

        public UsuarioModel Cadastrar(UsuarioModel usuario)
        {
            usuario.DataCadastro = DateTime.Now;
            _projetoEstagioContext.Usuarios.Add(usuario);
            _projetoEstagioContext.SaveChanges();
            return usuario;
        }

        public List<UsuarioModel> ListarTodos()
        {
            return _projetoEstagioContext.Usuarios.ToList();
        }

        public UsuarioModel BuscarPorId(int id)
        {
            return _projetoEstagioContext.Usuarios.FirstOrDefault(u => u.Id == id);
        }

        public UsuarioModel Editar(UsuarioModel usuario)
        {
            _projetoEstagioContext.Usuarios.Update(usuario);
            _projetoEstagioContext.SaveChanges();
            return usuario;
        }

        public UsuarioModel Atualizar(UsuarioModel usuario)
        {
            UsuarioModel usuarioDB = BuscarPorId(usuario.Id);

            if (usuarioDB == null) throw new Exception("Erro na atualização. Usuário não encontrado!");

            usuarioDB.Login = usuario.Login;
            usuarioDB.Email = usuario.Email;
            usuarioDB.Perfil = usuario.Perfil;
            usuarioDB.DataAtualizacao = DateTime.Now;

            _projetoEstagioContext.Usuarios.Update(usuarioDB);
            _projetoEstagioContext.SaveChanges();

            return usuarioDB;
        }

        public bool Deletar(int id)
        {
            UsuarioModel usuarioDB = BuscarPorId(id);

            if (usuarioDB == null) throw new Exception("Erro ao deletar.");

            _projetoEstagioContext.Usuarios.Remove(usuarioDB);
            _projetoEstagioContext.SaveChanges();

            return true;
        }

        public UsuarioModel BuscarPorLogin(string login)
        {
            return _projetoEstagioContext.Usuarios.FirstOrDefault(x => x.Login == login);
        }
    }
}
