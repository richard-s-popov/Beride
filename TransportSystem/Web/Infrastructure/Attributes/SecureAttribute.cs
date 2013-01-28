using System.Web;
using System.Web.Mvc;

namespace TransportSystem.Area.Web.Infrastructure.Attributes
{
    public class SecureAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var rres = new RedirectResult("~/login");
                filterContext.Result = rres;
            }
        }
    }
}