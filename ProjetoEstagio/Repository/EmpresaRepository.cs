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
        // Cole isto dentro da classe EmpresaRepository
        public EmpresaModel Atualizar(EmpresaModel empresa)
        {
            // 1. Busca a empresa original no banco de dados
            EmpresaModel empresaDB = BuscarPorId(empresa.Id);

            if (empresaDB == null) throw new Exception("Erro na atualização. Empresa não encontrada!");

            // 2. Atualiza APENAS os campos permitidos
            empresaDB.RazaoSocial = empresa.RazaoSocial;
            empresaDB.Nome = empresa.Nome; // Nome Fantasia
            empresaDB.Email = empresa.Email;
            empresaDB.Telefone = empresa.Telefone;
            empresaDB.DataAtualizacao = DateTime.Now; // Atualiza a data da modificação

            // Campos como CNPJ, UsuarioId e DataCadastro NÃO são alterados

            // 3. Salva as mudanças
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
