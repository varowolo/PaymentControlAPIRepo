using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentControlAPI.MessageHandler
{
    public class JWTAuthHandler : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var _config = actionContext.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var auth = actionContext.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (auth == null)
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                actionContext.Result = new BadRequestObjectResult("Please provide token in the authorization header");
            }
            else if (!auth.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                actionContext.Result = new BadRequestObjectResult("Please user User Bearer Scheme before the token");
            }

            else
            {
                var token = new TokenManager(_config).ValidateToken(auth.Substring(7).ToString());
                if (token == null)
                {
                    actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    actionContext.Result = new BadRequestObjectResult("Token Expired or Invalid");
                }
                else
                {
                    HttpContext a = null;
                    GenericIdentity identity = new GenericIdentity(token.ToString());
                    Thread.CurrentPrincipal = new GenericPrincipal(identity, new string[] { token.ToString() });
                    actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    //   a.User = new GenericPrincipal(new GenericIdentity(token.ToString()), null);

                }
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
