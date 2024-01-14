using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Comic.API.Code.Middleware;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CachedAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _timeToLiveSeconds;

    public CachedAttribute(int timeToLiveSeconds)
    {
        _timeToLiveSeconds = timeToLiveSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var memoryCache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

        if (memoryCache == null)
        {
            throw new InvalidOperationException("IMemoryCache not available in the service provider.");
        }

        var cacheKey = GenerateCacheKey(context.HttpContext.Request);

        if (memoryCache.TryGetValue(cacheKey, out var cachedResult))
        {
            context.Result = new ContentResult
            {
                Content = cachedResult.ToString(),
                StatusCode = 200,
                ContentType = "application/json",
            };
            return;
        }
        var resultContext = await next();

        if (resultContext.Result is OkObjectResult objectResult && objectResult.Value != null)
        {
            var serializedResult = JsonConvert.SerializeObject(objectResult.Value, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            memoryCache.Set(cacheKey, serializedResult, TimeSpan.FromSeconds(_timeToLiveSeconds));
        }
    }

    private string GenerateCacheKey(HttpRequest request)
    {
        // Generate a unique key based on the request parameters
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{request.Path}");
        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}