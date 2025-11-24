using ProjetoEstagio.Models;
using ProjetoEstagio.Repository;
using ProjetoEstagio.Helper;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Data;

namespace ProjetoEstagio.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ProjetoEstagioContext _context;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmail _email;

        public UsuarioService(ProjetoEstagioContext context, IUsuarioRepository usuarioRepository, IEmail email)
        {
            _context = context;
            _usuarioRepository = usuarioRepository;
            _email = email;
        }

        public void AlterarSenha(AlterarSenhaViewModel viewModel)
        {
            // 1. Busca o usuário no banco
            var usuarioDB = _usuarioRepository.BuscarPorId(viewModel.Id);

            if (usuarioDB == null)
                throw new Exception("Usuário não encontrado.");

            // 2. Verifica se a SENHA ATUAL está correta
            // (Usa o método SenhaValida que já existe no seu Model)
            if (!usuarioDB.SenhaValida(viewModel.SenhaAtual))
            {
                throw new Exception("A senha atual está incorreta.");
            }

            // 3. Gera o hash da NOVA SENHA
            // (Usa o método SetSenhaHash que já existe no seu Model)
            usuarioDB.SetSenhaHash(viewModel.NovaSenha);
            usuarioDB.DataAtualizacao = DateTime.Now;

            // 4. Salva no banco
            _usuarioRepository.Atualizar(usuarioDB);
        }

        public UsuarioModel BuscarPorLogin(string login)
        {
            return _usuarioRepository.BuscarPorLogin(login);
        }

        public void RedefinirSenha(string login)
        {
            // 1. Busca o usuário
            UsuarioModel usuario = _usuarioRepository.BuscarPorLogin(login);

            if (usuario == null)
            {
                throw new Exception($"Não encontramos nenhum usuário com o login '{login}'.");
            }

            // 2. Gera e Criptografa a nova senha
            // (O método GerarNovaSenha do Model já faz o SetSenhaHash corretamente agora)
            string novaSenha = usuario.GerarNovaSenha();

            // 3. Monta o e-mail
            string mensagem = $"Sua nova senha temporária é: <b>{novaSenha}</b>";
            string assunto = "SGE - Redefinição de Senha";

            // 4. Tenta enviar o e-mail
            bool emailEnviado = _email.Enviar(usuario.Email, assunto, mensagem);

            if (emailEnviado)
            {
                // 5. Só salva no banco se o e-mail foi enviado
                _usuarioRepository.Atualizar(usuario);
            }
            else
            {
                throw new Exception("Não conseguimos enviar o e-mail. Tente novamente mais tarde.");
            }
        }

        public void CadastrarAdmin(AdminCadastroViewModel viewModel)
        {
            // Validações de Negócio
            if (_usuarioRepository.VerificarEmailUnico(viewModel.Email).Result)
            {
                throw new InvalidOperationException("O e-mail informado já está em uso.");
            }

            // Transação (Unit of Work)
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // A. Criar o Usuário (Acesso)
                    var usuario = new UsuarioModel
                    {
                        Login = viewModel.Email,
                        Email = viewModel.Email,
                        Perfil = Perfil.Admin
                    };
                    usuario.SetSenhaHash(viewModel.Senha);

                    _usuarioRepository.Cadastrar(usuario); // Salva e gera o ID

                    // B. Criar o Admin (Dados Pessoais)
                    var admin = new AdminModel
                    {
                        Nome = viewModel.Nome,
                        CPF = viewModel.CPF,
                        UsuarioId = usuario.Id // Vincula ao usuário criado acima
                    };

                    _context.Administradores.Add(admin);
                    _context.SaveChanges();

                    // C. Confirma tudo
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; // Relança o erro para o Controller exibir
                }
            }
        }
    }
}