// Models/OrientadorModel.cs
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class OrientadorModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string CPF { get; set; }

        public string? Telefone { get; set; }

        [Required]
        public string Email { get; set; }

        // Campo específico do Orientador (Ex: "Departamento de Tecnologia")
        [Required]
        public string Departamento { get; set; }

        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        // Chave estrangeira para o login
        public int UsuarioId { get; set; }
        public virtual UsuarioModel Usuario { get; set; }

        // Vínculo com os Termos que ele orienta
        public virtual ICollection<TermoCompromissoModel> TermosOrientados { get; set; }
    }
}