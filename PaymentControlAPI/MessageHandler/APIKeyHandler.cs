using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentControlAPI.MessageHandler
{
    public class APIKeyHandler : ActionFilterAttribute
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //   private IConfiguration _config { get; set; }
        StringValues requestHeaders;

        public override void OnActionExecuting(ActionExecutingContext actionContext)

        {
            var _config = actionContext.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

            // AzureSecretManager.GetSecret("APIKEY").msg;        //  _httpContextAccessor = actionContext.HttpContext.RequestServices.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var key = actionContext.HttpContext.Request.Headers.Where(o => o.Key == "APIKey").FirstOrDefault().Value.ToString();
            var tokencheck = actionContext.HttpContext.Request.Headers.TryGetValue("APIKey", out requestHeaders);
            if (tokencheck)
            {
                if (key.ToString() != _config.GetSection("APIKey").Value)
                {
                    actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    actionContext.Result = new BadRequestObjectResult("No valid APIKEY in the authorization header");
                }
                else
                {
                    var identity = new ClaimsIdentity();
                    identity.AddClaim(new Claim(ClaimTypes.Name, key));
                    Thread.CurrentPrincipal = new ClaimsPrincipal(identity);
                    actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;


                    //actionContext.Result = new OkObjectResult("Access Granted");

                }
            }
            else
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                actionContext.Result = new BadRequestObjectResult("No valid APIKEY in the authorization header");
            }
            base.OnActionExecuting(actionContext);
        }

    }
}
