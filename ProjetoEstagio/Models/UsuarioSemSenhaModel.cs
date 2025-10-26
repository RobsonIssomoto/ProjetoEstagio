using ProjetoEstagio.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class UsuarioSemSenhaModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo Login é de preenchimento Obrigatório.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "O campo E-mail é de preenchimento Obrigatório.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A seleção do Perfil é Obrigatória.")]
        public Perfil? Perfil { get; set; }

    }
}
