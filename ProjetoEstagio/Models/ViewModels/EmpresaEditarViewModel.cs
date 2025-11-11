using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class EmpresaEditarViewModel
    {
        // Precisamos do Id para saber quem atualizar
        public int Id { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; } // Apenas para exibir (readonly)

        [Required(ErrorMessage = "O campo Razão Social é obrigatório.")]
        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Required(ErrorMessage = "O campo Nome Fantasia é obrigatório.")]
        [Display(Name = "Nome Fantasia")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Insira um formato de e-mail válido.")]
        public string Email { get; set; }

        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }
    }
}