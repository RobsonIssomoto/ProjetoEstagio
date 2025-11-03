using ProjetoEstagio.Data;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public class SupervisorRepository : ISupervisorRepository
    {
        private readonly ProjetoEstagioContext _projetoEstagioContext;

        public SupervisorRepository(ProjetoEstagioContext projetoEstagioContext)
        {
            _projetoEstagioContext = projetoEstagioContext;
        }

        public SupervisorModel Cadastrar(SupervisorModel supervisor)
        {
            _projetoEstagioContext.Supervisores.Add(supervisor);
            _projetoEstagioContext.SaveChanges();
            return supervisor;
        }

        public List<SupervisorModel> ListarTodos()
        {
            return _projetoEstagioContext.Supervisores.ToList();
        }

        public List<SupervisorModel> ListarPorEmpresa(int empresaId)
        {
            return _projetoEstagioContext.Supervisores
                  .Where(s => s.EmpresaId == empresaId)
                  .ToList();
        }

        public SupervisorModel BuscarPorId(int id)
        {
            return _projetoEstagioContext.Supervisores.FirstOrDefault(s => s.Id == id);
        }

        public SupervisorModel Editar(SupervisorModel supervisor)
        {
            _projetoEstagioContext.Supervisores.Update(supervisor);
            _projetoEstagioContext.SaveChanges();
            return supervisor;
        }

        public SupervisorModel Atualizar(SupervisorModel supervisor)
        {
            SupervisorModel supervisorDB = BuscarPorId(supervisor.Id);

            if (supervisorDB == null) throw new Exception("Erro na atualização. Empresa não encontrada!");

            supervisorDB.Nome = supervisor.Nome;
            supervisorDB.CPF = supervisor.CPF;
            supervisorDB.Cargo = supervisor.Cargo;

            _projetoEstagioContext.Supervisores.Update(supervisorDB);
            _projetoEstagioContext.SaveChanges();

            return supervisorDB;
        }

        public bool Deletar(int id)
        {
            SupervisorModel supervisorDB = BuscarPorId(id);

            if (supervisorDB == null) throw new Exception("Erro ao deletar.");

            _projetoEstagioContext.Supervisores.Remove(supervisorDB);
            _projetoEstagioContext.SaveChanges();

            return true;
        }
    }
}
