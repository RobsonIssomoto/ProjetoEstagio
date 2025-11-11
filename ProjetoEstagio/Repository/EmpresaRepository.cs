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
            empresa.DataCadastro = DateTime.Now;
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

        public EmpresaModel BuscarPorUsuarioId(int usuarioId)
        {
            return _projetoEstagioContext.Empresas.FirstOrDefault(e => e.UsuarioId == usuarioId);
        }

        public EmpresaModel BuscarComSupervisores(int id)
        {
            // Usamos o Include para "incluir" a lista de Supervisores
            // na consulta da Empresa.
            return _projetoEstagioContext.Empresas
                .Include(e => e.Supervisores)
                .FirstOrDefault(e => e.Id == id);
        }

        // Dentro de EmpresaRepository.cs
        public EmpresaModel Atualizar(EmpresaModel empresa)
        {
            EmpresaModel empresaDB = BuscarPorId(empresa.Id);

            if (empresaDB == null) throw new Exception("Erro na atualização. Empresa não encontrada!");

            // Campos que serão atualizados:
            empresaDB.RazaoSocial = empresa.RazaoSocial;
            empresaDB.Nome = empresa.Nome; // Nome Fantasia
            empresaDB.Email = empresa.Email;
            empresaDB.Telefone = empresa.Telefone;
            empresaDB.DataAtualizacao = DateTime.Now; // Atualiza a data da modificação

            // Campos que NÃO serão atualizados:
            // empresaDB.CNPJ = ... (Não mexemos no CNPJ)
            // empresaDB.DataCadastro = ... (Não mexemos na data de cadastro)
            // empresaDB.UsuarioId = ... (Não mexemos no vínculo com o usuário)

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

        public async Task<bool> VerificarCNPJUnico(string cnpj)
        {
            return await _projetoEstagioContext.Empresas
                 .AnyAsync(e => e.CNPJ == cnpj.ToString());
        }
    }
}
