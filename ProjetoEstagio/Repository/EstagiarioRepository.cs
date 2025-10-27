using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public class EstagiarioRepository : IEstagiarioRepository
    {
        private readonly ProjetoEstagioContext _projetoEstagioContext;

        public EstagiarioRepository(ProjetoEstagioContext projetoEstagioContext)
        {
            _projetoEstagioContext = projetoEstagioContext;
        }

        public EstagiarioModel Cadastrar(EstagiarioModel estagiario)
        {
            _projetoEstagioContext.Estagiarios.Add(estagiario);
            _projetoEstagioContext.SaveChanges();
            return estagiario;
        }

        public List<EstagiarioModel> ListarTodos()
        {
            return _projetoEstagioContext.Estagiarios.ToList();
        }

        public EstagiarioModel BuscarPorId(int id)
        {
            return _projetoEstagioContext.Estagiarios.FirstOrDefault(e => e.Id == id);
        }

        public EstagiarioModel Editar(EstagiarioModel estagiario)
        {
            _projetoEstagioContext.Estagiarios.Update(estagiario);
            _projetoEstagioContext.SaveChanges();
            return estagiario;
        }

        public EstagiarioModel Atualizar(EstagiarioModel estagiario)
        {
            EstagiarioModel estagiarioDB = BuscarPorId(estagiario.Id);

            if (estagiarioDB == null) throw new Exception("Erro na atualização. Empresa não encontrada!");

            estagiarioDB.Nome = estagiario.Nome;
            estagiarioDB.Email = estagiario.Email;

            _projetoEstagioContext.Estagiarios.Update(estagiarioDB);
            _projetoEstagioContext.SaveChanges();

            return estagiarioDB;

        }

        public bool Deletar(int id)
        {
            EstagiarioModel estagiarioDB = BuscarPorId(id);

            if (estagiarioDB == null) throw new Exception("Erro ao deletar.");

            _projetoEstagioContext.Estagiarios.Remove(estagiarioDB);
            _projetoEstagioContext.SaveChanges();

            return true;
        }
    }
}
