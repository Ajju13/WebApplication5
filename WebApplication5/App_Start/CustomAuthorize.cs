using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;


namespace WebApplication5.App_Start
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedUserTypes;

        public CustomAuthorizeAttribute(params string[] userTypes)
        {
            this.allowedUserTypes = userTypes;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {


            var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var userType = ticket.UserData;

                if (allowedUserTypes.Contains(userType))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/Unauthorized");
        }
    }
}