using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface IEmpresaRepository
    {
        EmpresaModel Cadastrar(EmpresaModel empresa);
        List<EmpresaModel> ListarTodos();
        EmpresaModel BuscarPorId(int id);
        EmpresaModel BuscarComSupervisores(int id);
        EmpresaModel Editar(EmpresaModel empresa);
        EmpresaModel Atualizar(EmpresaModel empresa);
        bool Deletar(int id);
    }
}
