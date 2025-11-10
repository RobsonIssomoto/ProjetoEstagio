using ProjetoEstagio.Data;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Models;
using ProjetoEstagio.Repository;
using Microsoft.EntityFrameworkCore;

namespace ProjetoEstagio.Services
{
    public class EmpresaService : IEmpresaService
    {
        // 4. Dependências Injetadas
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISolicitacaoEstagioRepository _solicitacaoRepository;
        private readonly ITermoCompromissoRepository _termoRepository;
        private readonly ProjetoEstagioContext _context;

        public EmpresaService(
            IEmpresaRepository empresaRepository,
            IUsuarioRepository usuarioRepository,
            ISolicitacaoEstagioRepository solicitacaoRepository,
            ITermoCompromissoRepository termoRepository,
            ProjetoEstagioContext context) // Adicionado
        {
            _empresaRepository = empresaRepository;
            _usuarioRepository = usuarioRepository;
            _solicitacaoRepository = solicitacaoRepository;
            _termoRepository = termoRepository;
            _context = context;
        }

        public EmpresaModel Atualizar(EmpresaModel empresa)
        {
            return _empresaRepository.Atualizar(empresa);
        }

        public EmpresaModel BuscarComSupervisores(int id)
        {
            return _empresaRepository.BuscarComSupervisores(id);
        }

        public EmpresaModel BuscarPorId(int id)
        {
            return _empresaRepository.BuscarPorId(id);
        }

        public EmpresaModel BuscarPorUsuarioId(int usuarioId)
        {
            return _empresaRepository.BuscarPorUsuarioId(usuarioId);
        }

        public EmpresaModel BuscarEmpresaPorUsuarioId(int usuarioId)
        {
            return _empresaRepository.BuscarPorUsuarioId(usuarioId);
        }

        public List<SolicitacaoEstagioModel> ListarSolicitacoes(int empresaId)
        {
            return _solicitacaoRepository.ListarPorEmpresaId(empresaId);
        }

        public EmpresaModel Cadastrar(EmpresaModel empresa)
        {
            throw new NotImplementedException();
        }

        public bool Deletar(int id)
        {
            return _empresaRepository.Deletar(id);
        }

        public List<EmpresaModel> ListarTodos()
        {
            return _empresaRepository.ListarTodos();
        }

        // Dentro da sua classe EmpresaService
        public void RegistrarNovaEmpresa(EmpresaCadastroViewModel viewModel, string? token)
        {
            // === VERIFICAÇÃO PRÉVIA ===
            // (Assumindo que seus repositórios têm os métodos de verificação)
            if (_usuarioRepository.VerificarEmailUnico(viewModel.Email).Result)
            {
                throw new InvalidOperationException("O e-mail informado já está em uso.");
            }

            if (_empresaRepository.VerificarCNPJUnico(viewModel.CNPJ).Result)
            {
                throw new InvalidOperationException("O CNPJ informado já está em uso.");
            }

            // === TRANSAÇÃO ===
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Criar o Usuario
                    var usuario = new UsuarioModel
                    {
                        Login = viewModel.Email,
                        Email = viewModel.Email,
                        Perfil = Perfil.Representante
                    };
                    usuario.SetSenhaHash(viewModel.Senha);
                    _usuarioRepository.Cadastrar(usuario);

                    // 2. Criar a Empresa
                    var empresa = new EmpresaModel
                    {
                        RazaoSocial = viewModel.RazaoSocial,
                        CNPJ = viewModel.CNPJ,
                        Nome = viewModel.Nome,
                        Telefone = viewModel.Telefone,
                        Email = viewModel.Email,
                        UsuarioId = usuario.Id
                    };
                    _empresaRepository.Cadastrar(empresa); // Salva e obtém o empresa.Id

                    // --- 3. LÓGICA DE VÍNCULO CORRIGIDA ---
                    // A variável 'token' agora é reconhecida
                    if (!string.IsNullOrEmpty(token))
                    {
                        // O método 'BuscarPorToken' agora existe
                        var solicitacao = _solicitacaoRepository.BuscarPorToken(token);

                        // Usando os enums corretos
                        if (solicitacao != null && solicitacao.Status == Status.Aguardando)
                        {
                            solicitacao.EmpresaId = empresa.Id;
                            solicitacao.Status = Status.Pendente;
                            solicitacao.Token = null; // Invalida o token

                            // O método 'Atualizar' agora existe
                            _solicitacaoRepository.Atualizar(solicitacao);
                        }
                    }

                    // 4. Salva tudo
                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public SolicitacaoEstagioModel BuscarSolicitacaoPorId(int solicitacaoId)
        {
            // Repassa a chamada para o repositório que já tem esse método
            return _solicitacaoRepository.BuscarPorId(solicitacaoId);
        }

        
        public void RejeitarTermo(TermoPreenchimentoViewModel viewModel, int empresaIdLogada)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var termoDB = _termoRepository.BuscarCompletoPorId(viewModel.TermoId);
                    if (termoDB == null) throw new Exception("Termo não encontrado.");
                    if (termoDB.SolicitacaoEstagio.EmpresaId != empresaIdLogada) throw new Exception("Acesso negado.");

                    // 1. Salva a justificativa no Termo
                    termoDB.Justificativa = viewModel.Justificativa;
                    _termoRepository.Atualizar(termoDB); // Salva a justificativa

                    // 2. Rejeita a Solicitação
                    var solicitacao = _solicitacaoRepository.BuscarPorId(viewModel.SolicitacaoId);
                    solicitacao.Status = Status.Rejeitado;
                    _solicitacaoRepository.Atualizar(solicitacao);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void SalvarTermo(TermoPreenchimentoViewModel viewModel, int empresaIdLogada)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Busca o Termo COMPLETO para a verificação de segurança
                    var termoDB = _termoRepository.BuscarCompletoPorId(viewModel.TermoId);

                    if (termoDB == null)
                    {
                        throw new Exception("Termo não encontrado.");
                    }

                    // 2. Verificação de Segurança (Propriedade)
                    if (termoDB.SolicitacaoEstagio.EmpresaId != empresaIdLogada)
                    {
                        throw new Exception("Acesso negado.");
                    }

                    // 3. Atualiza os dados do Termo
                    termoDB.CargaHoraria = viewModel.CargaHoraria;
                    termoDB.ValorBolsa = viewModel.ValorBolsa;
                    termoDB.DataInicio = viewModel.DataInicio;
                    termoDB.DataFim = viewModel.DataFim;
                    termoDB.NumeroApolice = viewModel.NumeroApolice;
                    termoDB.NomeSeguradora = viewModel.NomeSeguradora;
                    termoDB.Justificativa = null; // Limpa a justificativa, pois está aprovando

                    _termoRepository.Atualizar(termoDB);

                    // 4. Atualiza a Solicitação para "Aprovado"
                    // Não precisamos buscar de novo, já temos a solicitação no termoDB
                    var solicitacao = termoDB.SolicitacaoEstagio;
                    solicitacao.Status = Status.Aprovado;
                    _solicitacaoRepository.Atualizar(solicitacao);

                    // 5. Salva tudo
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public TermoCompromissoModel BuscarOuCriarTermoPorSolicitacao(int solicitacaoId, int empresaIdLogada)
        {
            // 1. Busca a solicitação E o termo (se já existir)
            var solicitacao = _context.SolicitacoesEstagio
                                    .Include(s => s.TermoCompromisso)
                                    .FirstOrDefault(s => s.Id == solicitacaoId);

            if (solicitacao == null)
            {
                throw new Exception("Solicitação não encontrada.");
            }

            // 2. Verificação de Segurança
            // (Buscamos a solicitação de novo, sem includes, só para checar o ID da empresa)
            var solicitacaoParaSeguranca = _solicitacaoRepository.BuscarPorId(solicitacaoId);
            if (solicitacaoParaSeguranca.EmpresaId != empresaIdLogada)
            {
                throw new Exception("Acesso negado. Esta solicitação não pertence à sua empresa.");
            }

            int termoId; // Variável para guardar o ID do termo

            // 3. Lógica "Get or Create"
            if (solicitacao.TermoCompromisso != null)
            {
                // Se o termo JÁ EXISTE, apenas pegue o ID
                termoId = solicitacao.TermoCompromisso.Id;
            }
            else
            {
                // Se o termo NÃO EXISTE, crie-o agora
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Muda o status da solicitação
                        solicitacao.Status = Status.EmAndamento;
                        _solicitacaoRepository.Atualizar(solicitacao);

                        // Cria o novo Termo de Compromisso
                        var novoTermo = new TermoCompromissoModel
                        {
                            SolicitacaoEstagioId = solicitacaoId,
                            // (Campos como CargaHoraria, etc. ficam nulos)
                        };

                        _termoRepository.Cadastrar(novoTermo); // Salva o termo
                        transaction.Commit();

                        // Pega o ID do termo que acabou de ser criado
                        termoId = novoTermo.Id;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            // --- ESTA É A CORREÇÃO ---
            // Em vez de retornar o "novoTermo" (vazio) ou o "solicitacao.TermoCompromisso" (incompleto),
            // nós usamos o 'termoId' (seja ele o antigo ou o novo) para buscar
            // o objeto COMPLETO que o controller precisa.

            var termoCompleto = _termoRepository.BuscarCompletoPorId(termoId);
            return termoCompleto;
        }
        public async Task<bool> VerificarCNPJUnico(string cnpj)
        {
            return await _empresaRepository.VerificarCNPJUnico(cnpj);
        }
    }
}
