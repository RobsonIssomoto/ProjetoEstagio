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
            try
            {
                estagiario.DataCadastro = DateTime.Now;
                _projetoEstagioContext.Estagiarios.Add(estagiario);
                _projetoEstagioContext.SaveChanges();
                return estagiario;
            }
            // Esta é a exceção específica para erros de salvamento do EF
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                throw new Exception($"Erro ao salvar no banco: {ex.InnerException?.Message}", ex);
            }
            catch (Exception ex)
            {
                // Captura qualquer outro erro que possa ter ocorrido antes do SaveChanges
                throw new Exception($"Erro inesperado: {ex.Message}", ex);
            }
        }

        public List<EstagiarioModel> ListarTodos()
        {
            return _projetoEstagioContext.Estagiarios.ToList();
        }


        public EstagiarioModel BuscarPorId(int id)
        {
            return _projetoEstagioContext.Estagiarios.FirstOrDefault(e => e.Id == id);
        }

        public EstagiarioModel Atualizar(EstagiarioModel estagiario)
        {
            // 1. Busca o estagiário original do banco
            EstagiarioModel estagiarioDB = BuscarPorId(estagiario.Id);

            if (estagiarioDB == null) throw new Exception("Erro na atualização. Estagiário não encontrado!");

            // 2. Copia os valores do objeto "estagiario" (vindo do controller)
            //    para o objeto "estagiarioDB" (vindo do banco)
            estagiarioDB.Nome = estagiario.Nome;
            estagiarioDB.Email = estagiario.Email;
            estagiarioDB.Telefone = estagiario.Telefone;
            estagiarioDB.NomeCurso = estagiario.NomeCurso;
            estagiarioDB.DataAtualizacao = DateTime.Now;

            // 3. Salva o objeto "estagiarioDB" que foi modificado
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

        public EstagiarioModel BuscarPorUsuarioId(int usuarioId)
        {
            // CORRETO: Compara a Chave Estrangeira (UsuarioId) 
            // com o ID do usuário (usuarioId)
            return _projetoEstagioContext.Estagiarios.FirstOrDefault(e => e.UsuarioId == usuarioId);
        }

        public async Task<bool> VerificarCPFUnico(string cpf)
        {
            // Verifica se já existe um ESTAGIÁRIO com este CPF
            return await _projetoEstagioContext.Estagiarios
                                    .AnyAsync(e => e.CPF == cpf); // ou e.CPF == cpfLimpo
        }
    }
}
