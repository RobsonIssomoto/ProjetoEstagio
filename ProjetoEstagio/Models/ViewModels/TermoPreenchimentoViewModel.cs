// Models/ViewModels/TermoPreenchimentoViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [Display(Name = "Razão Social")] // <-- CORREÇÃO DO NOME
        public string EmpresaNome { get; set; }

        [Display(Name = "Orientador")]
        public string OrientadorNome { get; set; } = "Aguardando designação";

        // --- Dados do Formulário (Preenchidos pela Empresa) ---
        [Range(20, 30, ErrorMessage = "A Carga Horária deve ser um valor positivo (ex: 30).")]
        [Display(Name = "Carga Horária Semanal")]
        public int CargaHoraria { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "O Valor da Bolsa não pode ser negativo.")]
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

        [Required(ErrorMessage = "É obrigatório selecionar um supervisor.")]
        [Display(Name = "Supervisor Responsável (da Empresa)")]
        public int? SupervisorId { get; set; } // Guarda o ID do supervisor selecionado

        // Esta lista será preenchida pelo Controller
        [ValidateNever]
        public SelectList SupervisoresDisponiveis { get; set; }

        [Required(ErrorMessage = "O Plano de Atividades é obrigatório.")]
        [Display(Name = "Plano de Atividades de Estágio (PAE)")]
        [DataType(DataType.MultilineText)]
        public string PlanoDeAtividades { get; set; }

        [Display(Name = "Justificativa (em caso de rejeição)")]
        public string? Justificativa { get; set; }
    }
}