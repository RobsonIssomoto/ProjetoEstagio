namespace ProjetoEstagio.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Métricas Principais
        public int TotalEstudantes { get; set; }
        public int TotalEmpresas { get; set; }
        public int EstagiosAtivos { get; set; } // Status = EmAndamento
        public int ValidacoesPendentes { get; set; } // Status = Aprovado (pela empresa)

        // Você pode adicionar mais métricas aqui no futuro
        // Ex: public int TotalSupervisores { get; set; }
    }
}