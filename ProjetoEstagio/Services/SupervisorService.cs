using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Data;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using ProjetoEstagio.Models.ViewModels;
using ProjetoEstagio.Repository;
using ProjetoEstagio.Services;

public class SupervisorService : ISupervisorService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISupervisorRepository _supervisorRepository;
    private readonly ProjetoEstagioContext _context;

    public SupervisorService(
        IUsuarioRepository usuarioRepository,
        ISupervisorRepository supervisorRepository,
        ProjetoEstagioContext context)
    {
        _usuarioRepository = usuarioRepository;
        _supervisorRepository = supervisorRepository;
        _context = context;
    }

    public void AtualizarSupervisor(SupervisorModel supervisor)
    {
        // No futuro, se a atualização ficar complexa (ex: checar o Usuário),
        // a lógica já estará no lugar certo.
        _supervisorRepository.Atualizar(supervisor);
    }

    public SupervisorModel BuscarPorId(int id)
    {
        return _supervisorRepository.BuscarPorId(id);
    }

    public void DeletarSupervisor(int supervisorId)
    {
        // Se deletar um supervisor precisar deletar o Usuário junto,
        // essa lógica (transacional) viria aqui.
        _supervisorRepository.Deletar(supervisorId);
    }

    public List<SupervisorModel> ListarPorEmpresa(int empresaId)
    {
        return _supervisorRepository.ListarPorEmpresa(empresaId);
    }

    public List<SupervisorModel> ListarTodos()
    {
        return _supervisorRepository.ListarTodos();
    }

    public void RegistrarNovoSupervisor(SupervisorCadastroViewModel viewModel, int empresaId)
    {
        // Validações de negócio (embora o [Remote] já deva ter pego)
        if (_usuarioRepository.VerificarEmailUnico(viewModel.Email).Result)
        {
            throw new InvalidOperationException("O e-mail informado já está em uso.");
        }
        // ... (você pode adicionar a verificação de CPF aqui também)

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                // 1. Criar o Usuário
                var usuario = new UsuarioModel
                {
                    Login = viewModel.Email,
                    Email = viewModel.Email,
                    Perfil = Perfil.Supervisor // Define o perfil!
                };
                usuario.SetSenhaHash(viewModel.Senha); // Cria o hash da senha

                _usuarioRepository.Cadastrar(usuario); // Salva o usuário no banco

                // 2. Criar o Supervisor
                var supervisor = new SupervisorModel
                {
                    Nome = viewModel.Nome,
                    CPF = viewModel.CPF,
                    Telefone = viewModel.Telefone,
                    Email = viewModel.Email,
                    Cargo = viewModel.Cargo,

                    // 3. VINCULAR TUDO
                    UsuarioId = usuario.Id,  // Vínculo com a entidade Usuario
                    EmpresaId = empresaId      // Vínculo com a Empresa logada
                };

                _supervisorRepository.Cadastrar(supervisor); // Salva o supervisor

                transaction.Commit(); // Confirma a transação
            }
            catch (Exception)
            {
                transaction.Rollback(); // Desfaz tudo em caso de erro
                throw;
            }
        }
    }

    public async Task<bool> VerificarCPFUnico(string cpf)
    {
        return await _context.Supervisores.AnyAsync(s => s.CPF == cpf);
    }
}