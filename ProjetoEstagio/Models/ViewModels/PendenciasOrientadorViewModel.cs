// Models/ViewModels/PendenciasOrientadorViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoEstagio.Models;
using System.Collections.Generic;

namespace ProjetoEstagio.Models.ViewModels
{
    public class PendenciasOrientadorViewModel
    {
        // A lista de termos que aguardam um orientador
        public List<TermoCompromissoModel> TermosPendentes { get; set; }

        // A lista de orientadores para preencher o dropdown
        public SelectList OrientadoresDisponiveis { get; set; }

        // Para o formulário de atribuição (usaremos no POST)
        public int TermoIdSelecionado { get; set; }
        public int OrientadorIdSelecionado { get; set; }
    }
}