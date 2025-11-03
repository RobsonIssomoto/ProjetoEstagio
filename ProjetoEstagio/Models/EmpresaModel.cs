using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class EmpresaModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A Razão Social é de preenchimento Obrigatório.")]
        public string RazaoSocial { get; set; }

        [Required(ErrorMessage = "O CNPJ é de preenchimento Obrigatório.(model)")]
        public string CNPJ { get; set; }

        [Required(ErrorMessage = "O nome é de preenchimento Obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O Telefone é de preenchimento Obrigatório.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O campo E-mail é de preenchimento Obrigatório.")]
        [EmailAddress(ErrorMessage = "O E-mail informado não é válido.")]
        public string Email { get; set; }

        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public int UsuarioId { get; set; }
        public virtual UsuarioModel Usuario { get; set; }

        public virtual ICollection<SupervisorModel>? Supervisores { get; set; }

    }
}
