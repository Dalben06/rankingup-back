using Microsoft.AspNetCore.Mvc;
using RankingUp.Core.Domain;

namespace RankingUp.WebApp.API.Controllers
{
    public abstract class ApiController : ControllerBase
    {
        //protected readonly SessaoUsuario _sessaoUsuario;
        public ApiController()
        {
            //_sessaoUsuario = autenticacaoService.SessaoUsuario;
        }

        protected IActionResult HttpResponse<T>(RequestResponse<T> result) where T : class
        {

            if (result != null && (result?.Notificacoes?.HasNotifications ?? false))
            {
                return BadRequest(new
                {
                    success = false,
                    errors = result.Notificacoes.Notifications
                });
            }
            else if (result == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(new
                {
                    success = true,
                    data = result.Model
                });
            }
        }

        protected IActionResult HttpResponse(Notifiable notifications)
        {

            if (!notifications.Valid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = notifications.Notifications
                });
            }
            else
            {
                return NoContent();
            }
        }

        protected IActionResult HttpResponse(NoContentResponse model)
        {

            if (model.Notificacoes.HasNotifications)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = model.Notificacoes.Notifications
                });
            }
            else
            {
                return NoContent();
            }
        }
    }
}
