
using ProjetoEstagio.Data;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Repository;
using Microsoft.EntityFrameworkCore;

namespace ProjetoEstagio.Services
{
    public class EstagiarioService : IEstagiarioService
    {
        private readonly IEstagiarioRepository _estagiarioRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ProjetoEstagioContext _context;
        private readonly ISolicitacaoEstagioRepository _solicitacaoRepository;

        public EstagiarioService(
            IEstagiarioRepository estagiarioRepository,
            IUsuarioRepository usuarioRepository,
            ProjetoEstagioContext context,
            ISolicitacaoEstagioRepository solicitacaoRepository) // <-- Adicionado
        {
            _estagiarioRepository = estagiarioRepository; //
            _usuarioRepository = usuarioRepository; //
            _context = context; //
            _solicitacaoRepository = solicitacaoRepository; // <-- Adicionado
        }

        public List<EstagiarioModel> ListarTodos()
        {
            return _estagiarioRepository.ListarTodos();
        }

        public EstagiarioModel BuscarPorId(int id)
        {
            return _estagiarioRepository.BuscarPorId(id);
        }

        public EstagiarioModel BuscarPorUsuarioId(int usuarioId)
        {
            return _estagiarioRepository.BuscarPorUsuarioId(usuarioId);
        }

        public EstagiarioModel Atualizar(EstagiarioModel estagiario)
        {
            return _estagiarioRepository.Atualizar(estagiario);
        }

        public bool Deletar(int id)
        {
            return _estagiarioRepository.Deletar(id);
        }

        public void RegistrarNovoEstagiario(EstagiarioCadastroViewModel viewModel)
        {


            if (_usuarioRepository.VerificarEmailUnico(viewModel.Email).Result)
            {
                throw new InvalidOperationException("O e-mail informado já está em uso.");
            }

            if (_estagiarioRepository.VerificarCPFUnico(viewModel.CPF).Result)
            {
                throw new InvalidOperationException("O CPF informado já está em uso.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Criar o Objeto Usuario
                    var usuario = new UsuarioModel
                    {
                        Login = viewModel.Email,
                        Email = viewModel.Email,
                        Perfil = Perfil.Estagiario
                    };
                    usuario.SetSenhaHash(viewModel.Senha); // Seta o HASH

                    _usuarioRepository.Cadastrar(usuario); // Salva o usuário

                    // 2. Criar o Objeto Estagiario
                    var estagiario = new EstagiarioModel
                    {
                        Nome = viewModel.Nome,
                        CPF = viewModel.CPF,
                        Telefone = viewModel.Telefone,
                        Email = viewModel.Email,
                        NomeCurso = viewModel.NomeCurso,
                        UsuarioId = usuario.Id // <-- O VÍNCULO!
                    };

                    _estagiarioRepository.Cadastrar(estagiario); // Salva o perfil

                    // 3. Se tudo deu certo, salva as mudanças no banco
                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw; // Relança a exceção para o Controller
                }

            }
        }

        public async Task<bool> VerificarCPFUnico(string cpf)
        {
            return await _estagiarioRepository.VerificarCPFUnico(cpf);
        }

        public void CriarSolicitacao(SolicitacaoCadastroViewModel viewModel)
        {
            try
            {
                var novaSolicitacao = new SolicitacaoEstagioModel
                {
                    EstagiarioId = viewModel.EstagiarioId,
                    Observacao = viewModel.Observacao,
                    DataSubmissao = DateTime.Now
                };

                // Cenário A: Empresa selecionada
                if (viewModel.EmpresaId.HasValue && viewModel.EmpresaId > 0)
                {
                    novaSolicitacao.EmpresaId = viewModel.EmpresaId.Value;
                    novaSolicitacao.Status = Status.Pendente;
                }
                // Cenário B: Convite por e-mail
                else if (!string.IsNullOrEmpty(viewModel.EmailConvite))
                {
                    novaSolicitacao.EmpresaId = null;
                    novaSolicitacao.Email = viewModel.EmailConvite;
                    novaSolicitacao.Token = Guid.NewGuid().ToString();
                    novaSolicitacao.Status = Status.Aguardando;

                    // (Lógica de _emailService.EnviarConviteEmpresa(...) viria aqui)
                }
                else
                {
                    // Se nenhum dos dois foi fornecido, lança uma exceção
                    throw new InvalidOperationException("Você deve selecionar uma empresa ou convidar uma nova por e-mail.");
                }

                _solicitacaoRepository.Cadastrar(novaSolicitacao);
            }
            catch (Exception)
            {
                // Relança a exceção para o Controller poder tratá-la
                throw;
            }
        }
    }
}
