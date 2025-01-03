﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CityInfo.API;

public class HttpsOnlyAttribute : Attribute,IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.IsHttps)
        {
          context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}
