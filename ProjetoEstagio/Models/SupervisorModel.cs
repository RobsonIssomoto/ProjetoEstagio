using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoEstagio.Models
{
    public class SupervisorModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome é de preenchimento Obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é de preenchimento Obrigatório.")]
        [Remote(action: "VerificarCPFUnico", controller: "Supervisor", ErrorMessage = "Este CPF já está cadastrado.")]
        [ValidarCPF(ErrorMessage = "O CPF informado não é válido.")]
        public string CPF { get; set; }
        [Required(ErrorMessage = "O Telefone é de preenchimento Obrigatório.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Remote(action: "VerificarEmailUnico", controller: "Supervisor", ErrorMessage = "Este E-mail já está cadastrado.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O Cargo é de preenchimento Obrigatório.")]
        public string Cargo { get; set; }

        public int EmpresaId { get; set; }
        [ForeignKey("EmpresaId")]
        public EmpresaModel Empresa { get; set; }
    }
}
