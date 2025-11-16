using ProjetoEstagio.Models;

namespace ProjetoEstagio.Services
{
    public interface IArquivoService
    {
        // Recebe os bytes do PDF e o termo (para pegar IDs e nomes)
        // Retorna o (Nome do Arquivo, Caminho Relativo)
        (string NomeArquivo, string CaminhoRelativo) SalvarTermoDeCompromisso(byte[] arquivoPdf, TermoCompromissoModel termo);

        // (Futuramente, podemos adicionar: DeletarArquivo, etc.)
    }
}
