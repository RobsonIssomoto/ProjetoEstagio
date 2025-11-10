// Models/ViewModels/SolicitacaoCadastroViewModel.cs
namespace ProjetoEstagio.Models.ViewModels
{
    public class SolicitacaoCadastroViewModel
    {
        // --- Dados do Estagiário (para exibir na tela) ---
        public int EstagiarioId { get; set; }
        public string EstagiarioNome { get; set; }
        public string EstagiarioCurso { get; set; }

        // --- Dados da Solicitação (para enviar ao controller) ---

        // Vinculado ao <input type="hidden" id="empresa-id-selecionada">
        public int? EmpresaId { get; set; }

        // Vinculado ao <textarea>
        public string? Observacao { get; set; }

        // Para o cenário de convite (a ser usado com um modal)
        public string? EmailConvite { get; set; }
    }
}