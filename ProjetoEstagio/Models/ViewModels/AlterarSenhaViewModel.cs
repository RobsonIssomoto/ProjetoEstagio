using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class AlterarSenhaViewModel
    {
        public int Id { get; set; } // ID do Usuário

        [Required(ErrorMessage = "Digite a senha atual.")]
        [Display(Name = "Senha Atual")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Digite a nova senha.")]
        //[MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirmar Nova Senha")]
        public string ConfirmarNovaSenha { get; set; }
    }
}