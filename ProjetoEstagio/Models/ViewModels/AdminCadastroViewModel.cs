using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models.Validation; // Se tiver validação de CPF
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class AdminCadastroViewModel
    {
        [Required(ErrorMessage = "O Nome Completo é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [Display(Name = "CPF")]
        // [ValidarCPF(ErrorMessage = "CPF inválido.")] // Descomente se tiver o validador
        public string CPF { get; set; }

        [Required(ErrorMessage = "O E-mail (Login) é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Remote(action: "VerificarEmailUnico", controller: "Usuario", ErrorMessage = "Este E-mail já está cadastrado.")]
        [Display(Name = "E-mail (Login)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A Confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmarSenha { get; set; }
    }
}