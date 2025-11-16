// Models/ViewModels/EstagiarioDashboardViewModel.cs
using ProjetoEstagio.Models;
using System.Collections.Generic;

namespace ProjetoEstagio.Models.ViewModels
{
    public class EstagiarioDashboardViewModel
    {
        // A lista de solicitações (para a tabela)
        public List<SolicitacaoEstagioModel> Solicitacoes { get; set; }

        // Para os cartões estáticos
        public int ProcessosPendentesCount { get; set; }
        public int RelatoriosPendentesCount { get; set; } // (Usaremos no futuro)
        public int DocumentosPendentesCount { get; set; } // (Usaremos no futuro)

        // Para controlar o botão "Iniciar Processo"
        public bool PossuiProcessoAtivoOuPendente { get; set; }
    }
}