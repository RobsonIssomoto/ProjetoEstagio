using ProjetoEstagio.Models;

namespace ProjetoEstagio.Services
{
    public class ArquivoService : IArquivoService
    {
        // Precisamos saber onde fica a pasta 'wwwroot'
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArquivoService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public (string NomeArquivo, string CaminhoRelativo) SalvarTermoDeCompromisso(byte[] arquivoPdf, TermoCompromissoModel termo)
        {
            // 1. Define o nome da pasta (ex: "arquivos/termos")
            //    Usamos Path.Combine para funcionar em Windows e Linux
            string pastaDestinoRelativa = Path.Combine("arquivos", "termos");
            string pastaDestinoAbsoluta = Path.Combine(_webHostEnvironment.WebRootPath, pastaDestinoRelativa);

            // 2. Cria a pasta se ela não existir
            if (!Directory.Exists(pastaDestinoAbsoluta))
            {
                Directory.CreateDirectory(pastaDestinoAbsoluta);
            }

            // 3. Define um nome de arquivo único
            string estagiarioNome = termo.SolicitacaoEstagio.Estagiario.Nome.Split(' ')[0];
            string nomeArquivo = $"TCE_{estagiarioNome}_{termo.Id}_{Guid.NewGuid().ToString().Substring(0, 4)}.pdf";

            // 4. Define o caminho completo (Absoluto e Relativo)
            string caminhoArquivoAbsoluto = Path.Combine(pastaDestinoAbsoluta, nomeArquivo);
            // O caminho relativo é o que salvamos no banco
            string caminhoArquivoRelativo = Path.Combine(Path.DirectorySeparatorChar.ToString(), pastaDestinoRelativa, nomeArquivo);

            // 5. Salva o arquivo no disco
            File.WriteAllBytes(caminhoArquivoAbsoluto, arquivoPdf);

            // 6. Retorna os nomes para salvar no banco
            return (nomeArquivo, caminhoArquivoRelativo);
        }
    }
}
