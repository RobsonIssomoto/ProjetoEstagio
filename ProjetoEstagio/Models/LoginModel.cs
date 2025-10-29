using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O Login é de preenchimento Obrigatório.")]
        [EmailAddress(ErrorMessage = "O E-mail informado não é válido.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "A Senha é de preenchimento Obrigatório.")]
        public string Senha { get; set; }
    }
}
