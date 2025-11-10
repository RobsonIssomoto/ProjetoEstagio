using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class EstagiarioCadastroViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [Remote(action: "VerificarCPFUnico", controller: "Estagiario", ErrorMessage = "Este CPF já está cadastrado.")]
        [ValidarCPF(ErrorMessage = "O CPF informado não é válido.")]
        public string CPF { get; set; }

        public string? Telefone { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Remote(action: "VerificarEmailUnico", controller: "Usuario", ErrorMessage = "Este E-mail já está cadastrado.")]
        public string Email { get; set; }

        public string? NomeCurso { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A Confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; }
    }
}