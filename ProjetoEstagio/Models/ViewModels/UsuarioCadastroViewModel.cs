// Models/ViewModels/UsuarioCadastroViewModel.cs
using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models.Enums; // Para o enum Perfil
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class UsuarioCadastroViewModel
    {
        [Required(ErrorMessage = "O E-mail (Login) é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Remote(action: "VerificarEmailUnico", controller: "Usuario", ErrorMessage = "Este E-mail já está cadastrado.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A seleção do Perfil é Obrigatória.")]
        public Perfil? Perfil { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A Confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; }
    }
}