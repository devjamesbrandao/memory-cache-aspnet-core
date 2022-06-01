using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MemoryCache.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    // Key para salvar o cache
    private const string Key = "Cache";

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    // Injeção de Dependência do Memory Cache
    private readonly IMemoryCache _cache;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        // Verifica se há cache
        if(_cache.TryGetValue(Key, out WeatherForecast[] temps))
        {
            return temps;
        }

        var temperaturas = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();

        // Configura as opções para guardas as informações no cache
        var memoryCache = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10),
            SlidingExpiration = TimeSpan.FromSeconds(5)
        };

        // Adiciona as informações no cache
        _cache.Set(Key, temperaturas, memoryCache);

        return temperaturas;
    }
}
