using System.Text.Json;
using System.Text.Json.Serialization;
using CapitalGain.Application.Services;
using CapitalGain.Domain.Entities;
using CapitalGain.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
ConfigureServices(services);

var provider = services.BuildServiceProvider();
var calculator = provider.GetRequiredService<ICapitalGainCalculator>();

string? line;
while ((line = Console.ReadLine()) != null)
{
    JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    List<Operation>? operations = JsonSerializer.Deserialize<List<Operation>>(line, options);

    if (operations is null) continue;

    List<Dictionary<string, decimal>> result = calculator.ProcessOperations(operations);
    Console.WriteLine(JsonSerializer.Serialize(result));
}

void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<ICapitalGainCalculator, CapitalGainCalculator>();
}