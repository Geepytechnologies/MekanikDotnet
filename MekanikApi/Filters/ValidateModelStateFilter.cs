using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace MekanikApi.Api.Filters
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        private readonly ILogger<ValidateModelStateFilter> _logger;

        public ValidateModelStateFilter(ILogger<ValidateModelStateFilter> logger)
        {
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {

                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value.Errors.Select(x => x.ErrorMessage)
                    });

                var modelStateValues = context.ModelState
                .SelectMany(e => e.Value.AttemptedValue);

                var errorResponse = new
                {
                    StatusCode = 400,
                    Message = "One or more validation errors occurred.",
                    Result = errors
                };

                var actionArgumentsLog = context.ActionArguments
            .ToDictionary(
                kvp => kvp.Key,
                kvp => Newtonsoft.Json.JsonConvert.SerializeObject(kvp.Value)
            );

                using (LogContext.PushProperty("ValidationMiddleware", "ValidationError"))
                {
                    _logger.LogError("Validation errors: {msg}", Newtonsoft.Json.JsonConvert.SerializeObject(errors));
                    _logger.LogError("Model state values: {modelStateValues}", actionArgumentsLog);
                }



                context.Result = new JsonResult(errorResponse)
                {
                    StatusCode = 400
                }; ;
            }
        }
    }
}
