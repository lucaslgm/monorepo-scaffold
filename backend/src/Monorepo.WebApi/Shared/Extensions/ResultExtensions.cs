using CSharpFunctionalExtensions;
using Monorepo.Domain.Commons.Entities;
using Monorepo.WebApi.Configurations.Factories;
using Microsoft.AspNetCore.Mvc;

namespace   Monorepo.WebApi.Shared.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T, Error> result, HttpResponseFactory factory) => factory.MapResult(result);

    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Result<T, Error>> resultTask, HttpResponseFactory factory)
    {
        var result = await resultTask;
        return factory.MapResult(result);
    }
}
