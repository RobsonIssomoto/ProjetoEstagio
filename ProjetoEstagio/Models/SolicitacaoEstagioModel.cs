using ProjetoEstagio.Models.Enums;
namespace ProjetoEstagio.Models
{
    public class SolicitacaoEstagioModel
    {
        public int Id { get; set; }
        public DateTime DataSubmissao { get; set; }
        public string? Observacao { get; set; }
        public Status? Status { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public int EstagiarioId { get; set; }
        public int? EmpresaId { get; set; }
        public virtual EstagiarioModel Estagiario { get; set; }
        public virtual EmpresaModel Empresa { get; set; }
        public virtual TermoCompromissoModel TermoCompromisso { get; set; }
    }
}
