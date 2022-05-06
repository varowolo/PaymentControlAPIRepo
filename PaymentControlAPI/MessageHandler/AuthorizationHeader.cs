using PaymentControlAPI.Model;
using PaymentControlAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentControlAPI.MessageHandler
{
    public class LoginHeader : ActionFilterAttribute
    {
         private readonly IHttpContextAccessor _httpContextAccessor;
        private EPCCOSDBContext _context { get; set; }


        public override void OnActionExecuting(ActionExecutingContext actionContext)

        {
            _context = actionContext.HttpContext.RequestServices.GetService(typeof(EPCCOSDBContext)) as EPCCOSDBContext;

            var auth = actionContext.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (auth == null)
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                actionContext.Result = new BadRequestObjectResult("Please provide Basic authorization header");
            }
            else if (!auth.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                actionContext.Result = new BadRequestObjectResult("Please User Basic Scheme before the key");
            }

            else
            {
                var decodedauth = Encryptor.base64Decode(auth.Substring(6).ToString());

                string username = decodedauth.Substring(0, decodedauth.IndexOf(":"));
                string password = decodedauth.Substring(decodedauth.IndexOf(":") + 1);
                var encPass = Encryptor.EncodePasswordMd5(password);
                var user = _context._tblAppClient.Where(o => o.UserName == username && o.Password == encPass).FirstOrDefault();
                if (user == null)
                {
                    actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                    actionContext.Result = new BadRequestObjectResult("Invalid authorization header. it is meant to be base64 username:password");
                }

                else
                {
                    var identity = new ClaimsIdentity();
                    identity.AddClaim(new Claim(ClaimTypes.Name, username));
                    Thread.CurrentPrincipal = new ClaimsPrincipal(identity);
                    actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                }

            }
            base.OnActionExecuting(actionContext);
        }

    }
}
