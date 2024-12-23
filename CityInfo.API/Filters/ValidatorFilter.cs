using CityInfo.API.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CityInfo.API.Filters;

public class ValidatorFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //before hits the controller
        if (!context.ModelState.IsValid)
        {
            var errorsInModelState = context.ModelState.Where(x => x.Value.Errors.Count > 0)
           .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray());

            var errorResponse = new ErrorResponse();
            foreach (var error in errorsInModelState)
            {
                foreach (var subError in error.Value)
                {
                    var errorModel = new ErrorModel
                    {
                        FiledName = error.Key,
                        ErrorMessage = subError
                    };
                    errorResponse.Errors.Add(errorModel);
                }
            }
            context.Result = new BadRequestObjectResult(errorResponse);
            return;
        }

        await next();

        //after hits the controller
    }
}
