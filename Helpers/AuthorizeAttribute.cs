using System.Linq;
using System;
using System.Diagnostics.Contracts;
using AirandWebAPI.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (SkipAuthorization(context)) return;
        var user = (User)context.HttpContext.Items["User"];
        if (user == null)
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }

    private static bool SkipAuthorization(AuthorizationFilterContext actionContext)
    {
        Contract.Assert(actionContext != null);
        // return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
        //            || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        return actionContext.Filters.OfType<AllowAnonymousAttribute>().Any() 
                    || actionContext.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
    }
}