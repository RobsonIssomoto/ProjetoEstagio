using Newtonsoft.Json;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Helper
{
    public class Sessao : ISessao
    {
        private readonly IHttpContextAccessor _httpContext;

        public Sessao(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;   
        }
        public UsuarioModel BuscarSessaoDoUsuario()
        {
            string sessaoUsuario = _httpContext.HttpContext.Session.GetString("sessaoUsuarioLogado");

            if(string.IsNullOrEmpty(sessaoUsuario)) return null;
            return JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);

        }
        public void CriarSessaoDoUsuario(UsuarioModel usuario)
        {
            string valor = JsonConvert.SerializeObject(usuario);
            _httpContext.HttpContext.Session.SetString("sessaoUsuarioLogado", valor);
        }

    
        public void SalvarEmpresaIdNaSessao(int empresaId)
        {
            // Salva o ID como um Inteiro (Int32).
            _httpContext.HttpContext.Session.SetInt32("sessaoEmpresaId", empresaId);
        }

        public int? BuscarEmpresaIdDaSessao()
        {
            // Lê o ID como um Inteiro (Int32). Retorna 'null' se não encontrar.
            return _httpContext.HttpContext.Session.GetInt32("sessaoEmpresaId");
        }

        public void SalvarEstagiarioIdNaSessao(int estagiarioId)
        {
            // Salva o ID como um Inteiro (Int32).
            _httpContext.HttpContext.Session.SetInt32("sessaoEstagiarioId", estagiarioId);
        }

        public int? BuscarEstagiarioIdDaSessao()
        {
            // Lê o ID como um Inteiro (Int32). Retorna 'null' se não encontrar.
            return _httpContext.HttpContext.Session.GetInt32("sessaoEstagiarioId");
        }

        public void SalvarNomeExibicao(string nome)
        {
            // Salva o nome como uma string simples
            _httpContext.HttpContext.Session.SetString("sessaoNomeExibicao", nome);
        }

        public string BuscarNomeExibicao()
        {
            // Busca o nome. Retorna null se não existir.
            return _httpContext.HttpContext.Session.GetString("sessaoNomeExibicao");
        }

        // --- 3. ATUALIZE O MÉTODO 'REMOVER' ---

        public void RemoverSessaoDoUsuario()
        {
            _httpContext.HttpContext.Session.Remove("sessaoUsuarioLogado");
            _httpContext.HttpContext.Session.Remove("sessaoEmpresaId");
            _httpContext.HttpContext.Session.Remove("sessaoEstagiarioId");// <-- Limpa o ID da empresa também
            _httpContext.HttpContext.Session.Remove("sessaoNomeExibicao"); // Limpa o nome também
        }
    }
}
