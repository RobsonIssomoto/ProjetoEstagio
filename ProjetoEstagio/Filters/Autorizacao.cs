using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ProjetoEstagio.Models;
using ProjetoEstagio.Models.Enums;
using System.Linq;

namespace ProjetoEstagio.Filters
{
    // Nome da classe alterado para "Autorizacao" conforme solicitado
    public class Autorizacao : ActionFilterAttribute
    {
        private readonly Perfil[] _perfisAutorizados;

        // O construtor aceita múltiplos perfis (ex: Admin, Estagiario)
        public Autorizacao(params Perfil[] perfisAutorizados)
        {
            _perfisAutorizados = perfisAutorizados;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 1. Busca a sessão
            string sessaoUsuario = context.HttpContext.Session.GetString("sessaoUsuarioLogado");

            // 2. Se não tiver sessão, manda pro Login
            if (string.IsNullOrEmpty(sessaoUsuario))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
                return;
            }

            // 3. Tenta converter a sessão em Usuário
            UsuarioModel usuario = JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);

            if (usuario == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
                return;
            }

            // 4. A MÁGICA: Verifica se o perfil do usuário está na lista de permitidos
            // Se a lista _perfisAutorizados tiver itens E o perfil do usuário NÃO estiver nela -> Bloqueia
            if (_perfisAutorizados.Length > 0 && !_perfisAutorizados.Contains(usuario.Perfil.Value))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Restrito" }, { "action", "Index" } });
            }

            base.OnActionExecuting(context);
        }
    }
}