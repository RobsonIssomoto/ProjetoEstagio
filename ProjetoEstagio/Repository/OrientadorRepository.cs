// Repository/OrientadorRepository.cs
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;
using System.Linq;

namespace ProjetoEstagio.Repository
{
    public class OrientadorRepository : IOrientadorRepository
    {
        private readonly ProjetoEstagioContext _context;

        public OrientadorRepository(ProjetoEstagioContext context)
        {
            _context = context;
        }

        public OrientadorModel Cadastrar(OrientadorModel orientador)
        {
            orientador.DataCadastro = DateTime.Now;
            _context.Orientadores.Add(orientador);
            _context.SaveChanges();
            return orientador;
        }

        public List<OrientadorModel> ListarTodos()
        {
            return _context.Orientadores.AsNoTracking().ToList();
        }

        public OrientadorModel BuscarPorId(int id)
        {
            return _context.Orientadores.FirstOrDefault(o => o.Id == id);
        }

        public OrientadorModel BuscarPorUsuarioId(int usuarioId)
        {
            return _context.Orientadores.FirstOrDefault(o => o.UsuarioId == usuarioId);
        }

        public OrientadorModel Atualizar(OrientadorModel orientador)
        {
            OrientadorModel orientadorDB = BuscarPorId(orientador.Id);

            if (orientadorDB == null)
            {
                throw new Exception("Erro na atualização. Orientador não encontrado!");
            }

            orientadorDB.Nome = orientador.Nome;
            orientadorDB.CPF = orientador.CPF;
            orientadorDB.Email = orientador.Email;
            orientadorDB.Telefone = orientador.Telefone;
            orientadorDB.Departamento = orientador.Departamento;
            orientadorDB.DataAtualizacao = DateTime.Now;

            _context.Orientadores.Update(orientadorDB);
            _context.SaveChanges();

            return orientadorDB;
        }

        public bool Deletar(int id)
        {
            OrientadorModel orientadorDB = BuscarPorId(id);

            if (orientadorDB == null)
            {
                throw new Exception("Erro ao deletar. Orientador não encontrado.");
            }

            _context.Orientadores.Remove(orientadorDB);
            _context.SaveChanges();

            return true;
        }
    }
}