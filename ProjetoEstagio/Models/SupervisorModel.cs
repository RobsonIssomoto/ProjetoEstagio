using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Models.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoEstagio.Models
{
    public class SupervisorModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public string? Email { get; set; }
        public string Cargo { get; set; }
        public int UsuarioId { get; set; }
        public virtual UsuarioModel Usuario { get; set; }
        public int EmpresaId { get; set; }
        [ForeignKey("EmpresaId")]
        public EmpresaModel Empresa { get; set; }


    }
}
