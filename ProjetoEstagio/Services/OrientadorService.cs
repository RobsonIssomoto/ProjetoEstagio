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

        public async Task<bool> VerificarCPFUnico(string cpf)
        {
            // Valida direto no contexto para usar Async
            return await _context.Orientadores.AnyAsync(o => o.CPF == cpf);
        }
    }
}