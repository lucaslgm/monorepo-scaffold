using CSharpFunctionalExtensions;
using FluentValidation;
using Monorepo.Domain.Commons.Entities;
using Monorepo.WebApi.Configurations.Factories;
using Monorepo.WebApi.Shared.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Monorepo.WebApi.Configurations.Filters;

public class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var param = context.ActionArguments.Values
            .FirstOrDefault(v => v != null && (v.GetType().Name.EndsWith("Request") || v.GetType().Name.EndsWith("Command")));
        if (param == null)
        {
            await next();
            return;
        }

        var services = context.HttpContext.RequestServices;
        var validatorType = typeof(IValidator<>).MakeGenericType(param.GetType());

        if (services.GetService(validatorType) is IValidator validator)
        {
            var validationContext = new ValidationContext<object>(param);
            var result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));

                var validationResult = Result.Failure<string, Error>(Error.Validation(errors));

                var factory = serviceProvider.GetRequiredService<HttpResponseFactory>();

                context.Result =
                    validationResult.ToActionResult(factory);

                return;
            }
        }

        await next();
    }
}
