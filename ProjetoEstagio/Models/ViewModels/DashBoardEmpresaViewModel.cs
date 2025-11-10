// Models/ViewModels/DashboardEmpresaViewModel.cs
using ProjetoEstagio.Models;
using System.Collections.Generic;

namespace ProjetoEstagio.Models.ViewModels
{
    public class DashboardEmpresaViewModel
    {
        public int PedidosPendentesCount { get; set; }
        public IEnumerable<SolicitacaoEstagioModel> PedidosDeEstagio { get; set; }
    }
}