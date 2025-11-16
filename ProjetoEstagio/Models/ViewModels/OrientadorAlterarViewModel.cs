using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.ViewModels
{
    // Este é o ViewModel para o modal de alteração
    public class OrientadorAlterarViewModel
    {
        // O ID do Termo de Compromisso que será alterado
        public int TermoId { get; set; }

        [Display(Name = "Estagiário")]
        public string EstagiarioNome { get; set; }

        [Required(ErrorMessage = "É obrigatório selecionar o novo orientador.")]
        [Display(Name = "Novo Orientador")]
        public int NovoOrientadorId { get; set; } // Onde o valor do dropdown será guardado

        // A lista de todos os orientadores para preencher o dropdown
        [ValidateNever] // (Impede o erro "The SupervisoresDisponiveis field is required")
        public SelectList OrientadoresDisponiveis { get; set; }
    }
}