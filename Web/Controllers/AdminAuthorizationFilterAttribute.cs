using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    //public class AdminAuthorizationFilterAttribute : FilterAttribute, IAuthorizationFilter
    //{
    //    public void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        if (filterContext.HttpContext.Session["Admin"] == null)
    //        {
    //            filterContext.Result = new RedirectResult("/login");
    //        }
    //    }
    //}
    public class AdminAuthorizationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["AdminToken"]?.Value;

            if (string.IsNullOrEmpty(token))
            {
                filterContext.Result = new RedirectResult("/login");
                return;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"]));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer =  Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
                    ValidAudience = Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);
            }
            catch
            {
                filterContext.Result = new RedirectResult("/login");
            }
        }
    }
}