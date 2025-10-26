using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoEstagio.Models
{
    public class SupervisorModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo Nome é de preenchimento Obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo CPF é de preenchimento Obrigatório.")]
        public string CPF { get; set; }
        [Required(ErrorMessage = "O campo Cargo é de preenchimento Obrigatório.")]
        public string Cargo { get; set; }

        public int EmpresaId { get; set; }
        [ForeignKey("EmpresaId")]
        public EmpresaModel Empresa { get; set; }
    }
}
