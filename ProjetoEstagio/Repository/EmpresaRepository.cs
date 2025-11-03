using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly ProjetoEstagioContext _projetoEstagioContext;
        public EmpresaRepository(ProjetoEstagioContext projetoEstagioContext)
        {
            _projetoEstagioContext = projetoEstagioContext;
        }

        public EmpresaModel Cadastrar(EmpresaModel empresa)
        {
            _projetoEstagioContext.Empresas.Add(empresa);
            _projetoEstagioContext.SaveChanges();
            return empresa;
        }

        public List<EmpresaModel> ListarTodos()
        {
            return _projetoEstagioContext.Empresas.ToList();
        }

        public EmpresaModel BuscarPorId(int id)
        {
            return _projetoEstagioContext.Empresas.FirstOrDefault(e => e.Id == id);
        }

        public EmpresaModel BuscarComSupervisores(int id)
        {
            // Usamos o Include para "incluir" a lista de Supervisores
            // na consulta da Empresa.
            return _projetoEstagioContext.Empresas
                .Include(e => e.Supervisores)
                .FirstOrDefault(e => e.Id == id);
        }


        public EmpresaModel BuscarPorUsuarioId(int usuarioId)
        {
            return _projetoEstagioContext.Empresas.FirstOrDefault(e => e.UsuarioId == usuarioId);
        }


        public EmpresaModel Editar(EmpresaModel empresa)
        {
            _projetoEstagioContext.Empresas.Update(empresa);
            _projetoEstagioContext.SaveChanges();
            return empresa;
        }
        public EmpresaModel Atualizar(EmpresaModel empresa)
        {
            EmpresaModel empresaDB = BuscarPorId(empresa.Id);

            if (empresaDB == null) throw new Exception("Erro na atualização. Empresa não encontrada!");

            empresaDB.Nome = empresa.Nome;
            empresaDB.CNPJ = empresa.CNPJ;
            empresaDB.Email = empresa.Email;

            _projetoEstagioContext.Empresas.Update(empresaDB);
            _projetoEstagioContext.SaveChanges();

            return empresaDB;

        }
        public bool Deletar(int id)
        {
            EmpresaModel empresaDB = BuscarPorId(id);

            if (empresaDB == null) throw new Exception("Erro ao deletar.");

            _projetoEstagioContext.Empresas.Remove(empresaDB);
            _projetoEstagioContext.SaveChanges();

            return true;
        }    
    }
}
