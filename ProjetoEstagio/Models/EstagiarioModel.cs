using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class EstagiarioModel
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string? Telefone { get; set; }
        public string Email { get; set; }
        public string? NomeCurso { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public int UsuarioId { get; set; }
        public virtual UsuarioModel Usuario { get; set; }
        public virtual ICollection<SolicitacaoEstagioModel> Estagios { get; set; }
    }
}
