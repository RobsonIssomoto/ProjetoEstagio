using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class AdminEditarViewModel
    {
        public int Id { get; set; }

        // O Admin usa o E-mail como Login principal
        [Required(ErrorMessage = "O campo E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Insira um e-mail válido.")]
        [Display(Name = "E-mail (Login)")]
        public string Email { get; set; }

        // O Admin (UsuarioModel) não tem campo "Nome" no banco, 
        // mas tem "Login" e "Email". Geralmente são iguais.
        // Se quiser adicionar "Nome" ao banco, precisaria de uma Migration.
        // Por enquanto, vamos assumir que ele edita o Email/Login.
    }
}