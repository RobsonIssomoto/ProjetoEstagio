using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class SupervisorCadastroViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é Obrigatório.")]
        // Aponte para o método no SupervisorController
        [Remote(action: "VerificarCPFUnico", controller: "Supervisor", ErrorMessage = "Este CPF já está cadastrado.")]
        [ValidarCPF(ErrorMessage = "O CPF informado não é válido.")]
        public string CPF { get; set; }

        public string? Telefone { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        // Aponte para o método no UsuarioController
        [Remote(action: "VerificarEmailUnico", controller: "Usuario", ErrorMessage = "Este E-mail já está cadastrado.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O Cargo é obrigatório.")]
        public string Cargo { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A Confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; }
    }
}