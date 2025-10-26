using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class EmpresaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é de preenchimento Obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo CNPJ é de preenchimento Obrigatório.")]

        public string CNPJ { get; set; }

        [Required(ErrorMessage = "O campo E-mail é de preenchimento Obrigatório.")]
        [EmailAddress(ErrorMessage = "O E-mail informado não é válido.")]
        public string Email { get; set; }

        public virtual ICollection<SupervisorModel>? Supervisores { get; set; }

        //public string Segmento { get; set; }
        //public string Cep { get; set; }
        //public string Rua { get; set; }
        //public string Cidade { get; set; }
        //public string Estado { get; set; }
        //public string Representante { get; set; }
        //public string Funcao { get; set; }
        //public string CPF { get; set; }
        //public string Departamento { get; set; }
        //public string telefone { get; set; }
        //public string Supervisor { get; set; }
        //public string Cargo { get; set; }

    }
}
