using BCrypt.Net; // 1. ADICIONE ESTE USING
using ProjetoEstagio.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo Login é de preenchimento Obrigatório.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "O campo E-mail é de preenchimento Obrigatório.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A seleção do Perfil é Obrigatória.")]
        public Perfil? Perfil { get; set; }

        [Required(ErrorMessage = "O campo Senha é de preenchimento Obrigatório.")]
        public string Senha { get; private set; } // 2. SET FOI MUDADO PARA "private"
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        // Construtor vazio para o EF Core
        public UsuarioModel() { }

        // --- 3. MÉTODO NOVO PARA CRIAR O HASH ---
        public void SetSenhaHash(string senha)
        {
            if (string.IsNullOrEmpty(senha))
            {
                throw new Exception("A senha não pode ser vazia.");
            }
            // Gera o "sal" e o "hash" da senha
            Senha = BCrypt.Net.BCrypt.HashPassword(senha);
        }

        // --- 4. MÉTODO ANTIGO ATUALIZADO PARA USAR HASH ---
        // (O LoginController vai chamar este método)
        public bool SenhaValida(string senha)
        {
            if (string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(this.Senha))
            {
                return false;
            }
            // Compara a senha digitada com o hash salvo no banco
            return BCrypt.Net.BCrypt.Verify(senha, this.Senha);
        }
        public string GerarNovaSenha()
        {
            // 1. Gera a senha aleatória (texto plano)
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);

            // 2. Criptografa ela antes de salvar na propriedade 'Senha'
            SetSenhaHash(novaSenha);

            // 3. Retorna a senha em texto plano (para enviar por e-mail)
            return novaSenha;
        }
    }
}