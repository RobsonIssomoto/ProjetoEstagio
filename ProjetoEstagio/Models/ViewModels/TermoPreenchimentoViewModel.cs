// Models/ViewModels/TermoPreenchimentoViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    public class TermoPreenchimentoViewModel
    {
        // --- IDs Ocultos ---
        public int TermoId { get; set; }
        public int SolicitacaoId { get; set; }

        // --- Dados de Exibição (Desabilitados no form) ---
        [Display(Name = "Estagiário")]
        public string EstagiarioNome { get; set; }

        [Display(Name = "Empresa")]
        public string EmpresaNome { get; set; }

        [Display(Name = "Orientador")]
        public string OrientadorNome { get; set; } = "Aguardando designação";

        // --- Dados do Formulário (Preenchidos pela Empresa) ---

        [Required(ErrorMessage = "A Carga Horária é obrigatória.")]
        [Display(Name = "Carga Horária Semanal")]
        public int CargaHoraria { get; set; }

        [Required(ErrorMessage = "O Valor da Bolsa é obrigatório.")]
        [Display(Name = "Valor da Bolsa (R$)")]
        public double ValorBolsa { get; set; }

        [Required(ErrorMessage = "A Data de Início é obrigatória.")]
        [Display(Name = "Data de Início")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "A Data de Término é obrigatória.")]
        [Display(Name = "Data de Término")]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

        [Required(ErrorMessage = "O Nº da Apólice é obrigatório.")]
        [Display(Name = "Nº da Apólice de Seguro")]
        public string NumeroApolice { get; set; }

        [Required(ErrorMessage = "O Nome da Seguradora é obrigatório.")]
        [Display(Name = "Nome da Seguradora")]
        public string NomeSeguradora { get; set; }

        [Display(Name = "Justificativa (em caso de rejeição)")]
        public string? Justificativa { get; set; }
    }
}