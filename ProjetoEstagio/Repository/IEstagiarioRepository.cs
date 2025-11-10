using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface IEstagiarioRepository
    {
        EstagiarioModel Cadastrar(EstagiarioModel estagiario);
        List<EstagiarioModel> ListarTodos();
        EstagiarioModel BuscarPorId(int id);
        EstagiarioModel BuscarPorUsuarioId(int usuarioId);
        EstagiarioModel Atualizar(EstagiarioModel estagiario);
        Task<bool> VerificarCPFUnico(string cpf);
        bool Deletar(int id);
    }
}
