using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class RedefinirSenhaModel
    {

        [Required(ErrorMessage = "O E-mail é Obrigatório.")]
        [EmailAddress(ErrorMessage = "O E-mail informado não é válido.")]
        public string Login { get; set; }
    }
}
