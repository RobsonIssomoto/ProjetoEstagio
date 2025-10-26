using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.ViewComponents
{
    public class Menu: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string sessaoUsuario = HttpContext.Session.GetString("sessaoUsuarioLogado");
            if (string.IsNullOrEmpty(sessaoUsuario)) return Content(string.Empty);

            UsuarioModel usuario = JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);
            return View(usuario);
        }
    }
}
