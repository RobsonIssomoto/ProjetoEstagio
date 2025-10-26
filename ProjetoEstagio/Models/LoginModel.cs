using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O campo Login é de preenchimento Obrigatório.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "O campo Senha é de preenchimento Obrigatório.")]
        public string Senha { get; set; }
    }
}
