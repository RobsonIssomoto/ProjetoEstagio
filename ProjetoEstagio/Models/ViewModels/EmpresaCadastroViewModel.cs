using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class EmpresaCadastroViewModel
    {
        [Required(ErrorMessage = "A Razão Social é obrigatória.")]
        public string RazaoSocial { get; set; }

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [Remote(action: "VerificarCNPJUnico", controller: "Empresa", ErrorMessage = "Este CNPJ já está cadastrado.")]
        [ValidarCNPJ(ErrorMessage = "O CNPJ informado não é válido.")]
        [MinLength(14, ErrorMessage = "O CNPJ deve conter 14 caracteres")]
        public string CNPJ { get; set; }

        [Required(ErrorMessage = "O Nome do Representante é obrigatório.")]
        public string Nome { get; set; }

        public string? Telefone { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Remote(action: "VerificarEmailUnico", controller: "Usuario", ErrorMessage = "Este E-mail já está cadastrado.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A Confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; }
    }
}