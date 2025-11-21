using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class EstagiarioEditarViewModel
    {
        // O Id é necessário, mas ficará 'hidden' no formulário
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        [Display(Name = "CPF")]
        public string CPF { get; set; } // Apenas para exibição (readonly)

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Insira um formato de e-mail válido.")]
        [Display(Name = "Email de Contato")]
        public string Email { get; set; }

        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [Display(Name = "Curso")]
        public string? NomeCurso { get; set; }

        
    }
}