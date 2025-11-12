// Services/OrientadorService.cs
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Repository;
using System.Threading.Tasks;

namespace ProjetoEstagio.Services
{
    public class OrientadorService : IOrientadorService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IOrientadorRepository _orientadorRepository;
        private readonly ProjetoEstagioContext _context; // Para transações e 'AnyAsync'

        public OrientadorService(
            IUsuarioRepository usuarioRepository,
            IOrientadorRepository orientadorRepository,
            ProjetoEstagioContext context)
        {
            _usuarioRepository = usuarioRepository;
            _orientadorRepository = orientadorRepository;
            _context = context;
        }

        public void RegistrarNovoOrientador(OrientadorCadastroViewModel viewModel)
        {
            // Validações de negócio (redundância de segurança para o [Remote])
            if (_usuarioRepository.VerificarEmailUnico(viewModel.Email).Result)
            {
                throw new InvalidOperationException("O e-mail informado já está em uso.");
            }
            if (VerificarCPFUnico(viewModel.CPF).Result)
            {
                throw new InvalidOperationException("O CPF informado já está cadastrado.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Criar o Usuário
                    var usuario = new UsuarioModel
                    {
                        Login = viewModel.Email,
                        Email = viewModel.Email,
                        Perfil = Perfil.Orientador //
                    };
                    usuario.SetSenhaHash(viewModel.Senha); //

                    _usuarioRepository.Cadastrar(usuario); // Salva o usuário

                    // 2. Criar o Orientador
                    var orientador = new OrientadorModel
                    {
                        Nome = viewModel.Nome,
                        CPF = viewModel.CPF,
                        Telefone = viewModel.Telefone,
                        Email = viewModel.Email,
                        Departamento = viewModel.Departamento, //
                        UsuarioId = usuario.Id  // Vincula ao usuário
                    };

                    _orientadorRepository.Cadastrar(orientador); // Salva o orientador

                    transaction.Commit(); // Confirma tudo
                }
                catch (Exception)
                {
                    transaction.Rollback(); // Desfaz tudo
                    throw;
                }
            }
        }

        public void AtualizarOrientador(OrientadorModel orientador)
        {
            // Atualiza os dados do Orientador
            _orientadorRepository.Atualizar(orientador);

            // Opcional: Atualizar dados do Usuário (se o e-mail/login puder mudar)
            var usuario = _usuarioRepository.BuscarPorId(orientador.UsuarioId);
            if (usuario != null && usuario.Email != orientador.Email)
            {
                usuario.Email = orientador.Email;
                usuario.Login = orientador.Email;
                _usuarioRepository.Atualizar(usuario);
            }
        }

        public void DeletarOrientador(int orientadorId)
        {
            // Lógica transacional para garantir que o Orientador e o Usuário sejam deletados
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var orientador = _orientadorRepository.BuscarPorId(orientadorId);
                    if (orientador != null)
                    {
                        int usuarioId = orientador.UsuarioId;

                        // 1. Deleta o Orientador
                        _orientadorRepository.Deletar(orientadorId);

                        // 2. Deleta o Usuário associado
                        _usuarioRepository.Deletar(usuarioId);

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public OrientadorModel BuscarPorId(int id)
        {
            return _orientadorRepository.BuscarPorId(id);
        }

        public List<OrientadorModel> ListarTodos()
        {
            return _orientadorRepository.ListarTodos();
        }

        public List<TermoCompromissoModel> ListarTermosPendentesDeOrientador()
        {
            // Esta consulta busca todos os Termos que:
            // 1. Já foram 'Aprovados' pela empresa (Status.Aprovado).
            // 2. Ainda não têm um 'OrientadorId' (OrientadorId == null).
            var termosPendentes = _context.TermosCompromisso
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Estagiario) // Traz o nome do Estagiário
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Empresa) // Traz o nome da Empresa
                .Where(t => t.OrientadorId == null && t.SolicitacaoEstagio.Status == Status.Aprovado)
                .AsNoTracking()
                .ToList();

            return termosPendentes;
        }

        // --- IMPLEMENTAÇÃO DO PASSO 2 (ATRIBUIR O ORIENTADOR) ---

        public void AtribuirOrientador(int termoId, int orientadorId)
        {
            // Usamos uma transação para garantir que a atribuição
            // e a mudança de status ocorram juntas.
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Busca o Termo E a Solicitação vinculada
                    var termo = _context.TermosCompromisso
                        .Include(t => t.SolicitacaoEstagio)
                        .FirstOrDefault(t => t.Id == termoId);

                    if (termo == null)
                    {
                        throw new Exception("Termo de Compromisso não encontrado.");
                    }

                    // 2. (Opcional, mas recomendado) Verifica se o Orientador existe
                    var orientador = _orientadorRepository.BuscarPorId(orientadorId);
                    if (orientador == null)
                    {
                        throw new Exception("Orientador selecionado não foi encontrado.");
                    }

                    // 3. ATRIBUI o orientador ao Termo
                    termo.OrientadorId = orientadorId;

                    // 4. ATUALIZA o Status da Solicitação
                    // O estágio agora sai de "Aprovado" (pela empresa)
                    // e vai para "Em Andamento" (aprovado pela instituição).
                    termo.SolicitacaoEstagio.Status = Status.EmAndamento;

                    // 5. Salva tudo
                    _context.SaveChanges(); // O EF Core é inteligente o bastante para salvar o Termo e a Solicitação
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; // Relança o erro para o Controller
                }
            }
        }

        public List<TermoCompromissoModel> ListarTodosOsTermos()
        {
            // Esta consulta busca TODOS os termos e
            // inclui todos os dados relacionados para a tabela.
            var todosOsTermos = _context.TermosCompromisso
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Estagiario) // Traz o Estagiário
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Empresa) // Traz a Empresa
                .Include(t => t.Orientador) // <-- Traz o Orientador (se houver)
                .AsNoTracking()
                .OrderByDescending(t => t.SolicitacaoEstagio.DataSubmissao) // Mais novos primeiro
                .ToList();

            return todosOsTermos;
        }
        public async Task<bool> VerificarCPFUnico(string cpf)
        {
            // Valida direto no contexto para usar Async
            return await _context.Orientadores.AnyAsync(o => o.CPF == cpf);
        }
    }
}