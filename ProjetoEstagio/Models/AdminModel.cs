using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoEstagio.Models
{
    public class AdminModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string CPF { get; set; } // Admin também é pessoa física

        // Chave estrangeira para o login (Igual ao EstagiarioModel)
        public int UsuarioId { get; set; }
        public virtual UsuarioModel Usuario { get; set; }
    }
}