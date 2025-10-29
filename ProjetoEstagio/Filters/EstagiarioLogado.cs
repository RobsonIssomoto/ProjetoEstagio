using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Filters
{
    public class EstagiarioLogado : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string sessaoEstagiario = context.HttpContext.Session.GetString("sessaoUsuarioLogado");

            if (string.IsNullOrEmpty(sessaoEstagiario))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { {"controller", "Login" }, {"action", "Index" } });
            }
            else
            {
                EstagiarioModel estagiario = JsonConvert.DeserializeObject<EstagiarioModel>(sessaoEstagiario);

                if (estagiario == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
                }
            }
                base.OnActionExecuting(context);
        }
    }
}
