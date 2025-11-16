using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoEstagio.Models
{
    public class TermoCompromissoModel
    {
        public int Id { get; set; }
        public int? CargaHoraria { get; set; }
        public double? ValorBolsa { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? NumeroApolice { get; set; }
        public string? NomeSeguradora { get; set; }
        public string? Justificativa { get; set; }
        public string? PlanoDeAtividades { get; set; }  
        [ForeignKey("Supervisor")]
        public int? SupervisorId { get; set; }
        public virtual SupervisorModel Supervisor { get; set; }
        public int SolicitacaoEstagioId { get; set; }
        public virtual SolicitacaoEstagioModel SolicitacaoEstagio { get; set; }
        [ForeignKey("Orientador")]
        public int? OrientadorId { get; set; }
        public virtual OrientadorModel Orientador { get; set; }
        public string? NomeArquivo { get; set; }
        public string? CaminhoArquivo { get; set; }
    }
}
