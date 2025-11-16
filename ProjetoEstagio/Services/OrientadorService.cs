// Services/OrientadorService.cs
using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Repository;
using System.Threading.Tasks;
using ProjetoEstagio.Documentos; // <-- 1. ADICIONE O USING DO DOCUMENTO
using QuestPDF.Fluent;           // <-- 2. ADICIONE O USING DO QUESTPDF

namespace ProjetoEstagio.Services
{
    public class OrientadorService : IOrientadorService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IOrientadorRepository _orientadorRepository;
        private readonly ProjetoEstagioContext _context;

        // --- 3. ADICIONE AS NOVAS DEPENDÊNCIAS ---
        private readonly ITermoCompromissoRepository _termoRepository;
        private readonly IArquivoService _arquivoService;
        private readonly ISolicitacaoEstagioRepository _solicitacaoRepository;

        // --- 4. ATUALIZE O CONSTRUTOR ---
        public OrientadorService(
            IUsuarioRepository usuarioRepository,
            IOrientadorRepository orientadorRepository,
            ProjetoEstagioContext context,
            ITermoCompromissoRepository termoRepository,     // Adicionado
            IArquivoService arquivoService,                   // Adicionado
            ISolicitacaoEstagioRepository solicitacaoRepository) // Adicionado
        {
            _usuarioRepository = usuarioRepository;
            _orientadorRepository = orientadorRepository;
            _context = context;
            _termoRepository = termoRepository;
            _arquivoService = arquivoService;
            _solicitacaoRepository = solicitacaoRepository;
        }

        // --- SEUS MÉTODOS EXISTENTES (COPIADOS DO SEU ARQUIVO) ---

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
                        Perfil = Perfil.Orientador
                    };
                    usuario.SetSenhaHash(viewModel.Senha);

                    _usuarioRepository.Cadastrar(usuario); // Salva o usuário

                    // 2. Criar o Orientador
                    var orientador = new OrientadorModel
                    {
                        Nome = viewModel.Nome,
                        CPF = viewModel.CPF,
                        Telefone = viewModel.Telefone,
                        Email = viewModel.Email,
                        Departamento = viewModel.Departamento,
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

            bool estaVinculado = _context.TermosCompromisso.Any(t => t.OrientadorId == orientadorId);

            if (estaVinculado)
            {
                // 2. Lança uma exceção "amigável". O OrientadorController 
                //    vai capturar isso (no catch) e o seu site.js 
                //    vai mostrar essa mensagem no alert() de erro.
                throw new InvalidOperationException("Este Orientador não pode ser excluído, pois ele já está vinculado a um ou mais Termos de Compromisso.");
            }

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
            // ... (Seu código existente, está correto) 
            var termosPendentes = _context.TermosCompromisso
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Estagiario)
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Empresa)
                .Where(t => t.OrientadorId == null && t.SolicitacaoEstagio.Status == Status.Aprovado)
                .AsNoTracking()
                .ToList();

            return termosPendentes;
        }


        // --- 5. MÉTODO "ATRIBUIRORIENTADOR" ATUALIZADO (O GATILHO) ---

        public void AtribuirOrientador(int termoId, int orientadorId)
        {
            // Usamos uma transação para garantir que a atribuição
            // e a geração do PDF ocorram juntas.
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Busca o Termo (sem includes, apenas para salvar o ID e os caminhos)
                    var termoParaSalvar = _termoRepository.BuscarPorId(termoId);
                    if (termoParaSalvar == null)
                    {
                        throw new Exception("Termo de Compromisso não encontrado.");
                    }

                    // 2. Busca a Solicitação vinculada
                    var solicitacao = _solicitacaoRepository.BuscarPorId(termoParaSalvar.SolicitacaoEstagioId);
                    if (solicitacao == null)
                    {
                        throw new Exception("Solicitação de estágio não encontrada.");
                    }

                    // 3. ATRIBUI o orientador ao Termo
                    termoParaSalvar.OrientadorId = orientadorId;

                    // 4. ATUALIZA o Status da Solicitação
                    solicitacao.Status = Status.EmAndamento;
                    _solicitacaoRepository.Atualizar(solicitacao); // Salva a mudança de status

                    // --- INÍCIO DA GERAÇÃO DO PDF ---

                    // 5. Busca o Termo COMPLETO (com todos os includes)
                    //    (Usando o método que atualizamos no ITermoCompromissoRepository)
                    var termoCompletoParaPdf = _termoRepository.BuscarCompletoPorId(termoId);

                    // 6. Atualiza o objeto completo com o OrientadorId que acabamos de atribuir
                    //    e busca o objeto Orientador para injetar no PDF
                    termoCompletoParaPdf.OrientadorId = orientadorId;
                    termoCompletoParaPdf.Orientador = _orientadorRepository.BuscarPorId(orientadorId);

                    // 7. Validação de dados (Supervisor deve existir)
                    if (termoCompletoParaPdf.Supervisor == null)
                    {
                        throw new Exception("Erro de dados: O Supervisor não foi encontrado para este termo.");
                    }

                    // 8. Instancia o template do QuestPDF
                    var documentoPdf = new TermoCompromissoDocument(termoCompletoParaPdf);

                    // 9. Gera o PDF em memória (bytes)
                    byte[] pdfBytes = documentoPdf.GeneratePdf();

                    // 10. Salva o PDF no disco (usando o ArquivoService)
                    (string nomeArquivo, string caminhoRelativo) = _arquivoService.SalvarTermoDeCompromisso(pdfBytes, termoCompletoParaPdf);

                    // 11. Salva os caminhos no banco de dados
                    termoParaSalvar.NomeArquivo = nomeArquivo;
                    termoParaSalvar.CaminhoArquivo = caminhoRelativo;
                    _termoRepository.Atualizar(termoParaSalvar); // Salva o Termo com o OrientadorId e os caminhos

                    // --- FIM DA GERAÇÃO DO PDF ---

                    // 12. Salva tudo no banco
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; // Relança o erro para o Controller
                }
            }
        }

        // --- SEUS MÉTODOS EXISTENTES (COPIADOS DO SEU ARQUIVO) ---

        public List<TermoCompromissoModel> ListarTodosOsTermos()
        {
            // ... (Seu código existente, está correto) 
            var todosOsTermos = _context.TermosCompromisso
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Estagiario)
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Empresa)
                .Include(t => t.Orientador)
                .AsNoTracking()
                .OrderByDescending(t => t.SolicitacaoEstagio.DataSubmissao)
                .ToList();

            return todosOsTermos;
        }

        public List<TermoCompromissoModel> ListarTermosPorOrientador(int orientadorId)
        {
            // ... (Seu código existente)
            var termos = _context.TermosCompromisso
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Estagiario)
                .Include(t => t.SolicitacaoEstagio)
                    .ThenInclude(s => s.Empresa)
                .Where(t => t.OrientadorId == orientadorId)
                .AsNoTracking()
                .OrderByDescending(t => t.SolicitacaoEstagio.DataSubmissao)
                .ToList();

            return termos;
        }

        // --- ADICIONE ESTA NOVA IMPLEMENTAÇÃO ---
        public void AlterarOrientadorDoTermo(int termoId, int novoOrientadorId)
        {
            // 1. Busca o termo original do banco (com tracking)
            var termoDB = _context.TermosCompromisso.FirstOrDefault(t => t.Id == termoId);

            if (termoDB == null)
            {
                throw new Exception("Termo de Compromisso não encontrado.");
            }

            // 2. Apenas altera o ID do orientador
            termoDB.OrientadorId = novoOrientadorId;

            // 3. Salva a mudança (não precisa do repositório, _context é mais direto)
            _context.SaveChanges();
        }

        public async Task<bool> VerificarCPFUnico(string cpf)
        {
            // ... (Seu código existente, está correto) [cite: 177-180]
            return await _context.Orientadores.AnyAsync(o => o.CPF == cpf);
        }
    }
}