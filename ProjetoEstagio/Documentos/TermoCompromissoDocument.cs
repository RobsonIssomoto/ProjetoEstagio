using ProjetoEstagio.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ProjetoEstagio.Documentos
{
    public class TermoCompromissoDocument : IDocument
    {
        private readonly TermoCompromissoModel _termo;
        private readonly EstagiarioModel _estagiario;
        private readonly EmpresaModel _empresa;
        private readonly SupervisorModel _supervisor;
        private readonly OrientadorModel _orientador;
        private readonly string _tipoEstagio;
        private readonly string _tipoRemuneracao;

        // Construtor (sem alteração, apenas pega os dados)
        public TermoCompromissoDocument(TermoCompromissoModel termo)
        {
            _termo = termo;
            _estagiario = _termo.SolicitacaoEstagio.Estagiario;
            _empresa = _termo.SolicitacaoEstagio.Empresa;
            _supervisor = _termo.Supervisor;
            _orientador = _termo.Orientador;

            // Lógica de Título
            bool isRemunerado = (_termo.ValorBolsa ?? 0) > 0;
            _tipoRemuneracao = isRemunerado ? "(REMUNERADO)" : "(NÃO REMUNERADO)";
            _tipoEstagio = "OBRIGATÓRIO"; // Assumindo por enquanto
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // --- MÉTODO COMPOSE SIMPLIFICADO ---
        // Removemos Header e Footer para garantir que funcione
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50); // Margem de 50 pontos

                    // Chama o método de conteúdo diretamente
                    page.Content().Element(ComposeContent);
                });
        }

        // --- MÉTODO DE CONTEÚDO (ÚNICO HELPER) ---
        // Todo o layout está aqui, usando apenas .Item() e .Text()
        void ComposeContent(IContainer container)
        {
            // Usar uma única coluna para o layout inteiro
            container.Column(col =>
            {
                // Título
                col.Item().AlignCenter().Text($"TERMO DE COMPROMISSO DE ESTÁGIO")
                    .Bold().FontSize(16);
                col.Item().AlignCenter().Text($"{_tipoEstagio} {_tipoRemuneracao}")
                    .Bold().FontSize(14);

                col.Item().PaddingBottom(20); // Adiciona um espaço

                // --- 1. DADOS DAS PARTES ---

                col.Item().Text("CONCEDENTE:").SemiBold();
                col.Item().Text($"{_empresa.RazaoSocial}, CNPJ: {_empresa.CNPJ}");
                col.Item().Text($"Representada por (Supervisor): {_supervisor.Nome} (Cargo: {_supervisor.Cargo})");
                col.Item().PaddingBottom(10); // Espaço

                col.Item().Text("ESTAGIÁRIO(A):").SemiBold();
                col.Item().Text($"{_estagiario.Nome}, CPF: {_estagiario.CPF}");
                col.Item().Text($"Curso: {_estagiario.NomeCurso}");
                col.Item().PaddingBottom(10); // Espaço

                col.Item().Text("INSTITUIÇÃO DE ENSINO:").SemiBold();
                col.Item().Text("Faculdade de Tecnologia de Atibaia (CNPJ: 62.823.257/0309-46");
                col.Item().Text($"Professor Orientador: {_orientador.Nome} (Depto: {_orientador.Departamento})");
                col.Item().PaddingBottom(20); // Espaço

                // --- 2. CLÁUSULAS (DADOS DO FORMULÁRIO) ---

                col.Item().Text("CLÁUSULA SEGUNDA - PLANO DE ATIVIDADES").SemiBold();
                // Simplesmente exibe o texto do plano de atividades
                col.Item().Text(_termo.PlanoDeAtividades);
                col.Item().PaddingBottom(10);

                col.Item().Text("CLÁUSULA TERCEIRA - CONDIÇÕES DO ESTÁGIO").SemiBold();
                col.Item().Text($"- Carga Horária: {_termo.CargaHoraria} horas semanais.");
                col.Item().Text($"- Vigência: De {_termo.DataInicio:dd/MM/yyyy} até {_termo.DataFim:dd/MM/yyyy}.");

                // Lógica simples para bolsa
                if ((_termo.ValorBolsa ?? 0) > 0)
                {
                    col.Item().Text($"- Bolsa-Auxílio: R$ {_termo.ValorBolsa:F2}");
                }
                else
                {
                    col.Item().Text("- Bolsa-Auxílio: Não remunerado.");
                }
                col.Item().PaddingBottom(10);

                col.Item().Text("CLÁUSULA SÉTIMA - SEGURO").SemiBold();
                col.Item().Text($"- Apólice Nº: {_termo.NumeroApolice}");
                col.Item().Text($"- Seguradora: {_termo.NomeSeguradora}");
                col.Item().PaddingBottom(20);

                // --- 3. ASSINATURAS ---

                col.Item().Text($"Atibaia, {DateTime.Now:dd 'de' MMMM 'de' yyyy}.");
                col.Item().PaddingTop(80); // Espaço grande para as assinaturas

                // Assinatura 1: Estagiário
                col.Item().AlignCenter().Text("________________________________________");
                col.Item().AlignCenter().Text($"{_estagiario.Nome}\nCPF: {_estagiario.CPF}\n(Estagiário(a))").SemiBold();
                col.Item().PaddingTop(35); // Espaço entre assinaturas

                // Assinatura 2: Supervisor
                col.Item().AlignCenter().Text("________________________________________");
                col.Item().AlignCenter().Text($"{_supervisor.Nome}\nCPF: {_supervisor.CPF}\n(Supervisor - Concedente)").SemiBold();
                col.Item().PaddingTop(35); // Espaço entre assinaturas

                // Assinatura 3: Orientador
                col.Item().AlignCenter().Text("________________________________________");
                col.Item().AlignCenter().Text($"{_orientador.Nome}\nCPF: {_orientador.CPF}\n(Orientador - Instituição)").SemiBold();
            });
        }
    }
}