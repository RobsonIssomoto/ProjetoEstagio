namespace ProjetoEstagio.Helper
{
    public interface IEmail
    {
        bool Enviar(string Email, string assuto, string mensagem);
    }
}
