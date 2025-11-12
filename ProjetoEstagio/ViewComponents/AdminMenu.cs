// ViewComponents/AdminMenu.cs
using Microsoft.AspNetCore.Mvc;
using ProjetoEstagio.Helper;
using System.Threading.Tasks;

namespace ProjetoEstagio.ViewComponents
{
    public class AdminMenu : ViewComponent
    {
        private readonly ISessao _sessao;

        // Injeta a sessão para pegar o nome do usuário
        public AdminMenu(ISessao sessao)
        {
            _sessao = sessao;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Busca o nome do usuário salvo no LoginController
            string nomeUsuario = _sessao.BuscarNomeExibicao();
            if (string.IsNullOrEmpty(nomeUsuario))
            {
                nomeUsuario = "Admin"; // Um fallback
            }

            return View("Default", nomeUsuario);
        }
    }
}