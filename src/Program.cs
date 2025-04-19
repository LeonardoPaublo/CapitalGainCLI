using System.Text.Json;
using System.Text.Json.Serialization;
using CapitalGain.Application.Services;
using CapitalGain.Domain.Entities;
using CapitalGain.Domain.Services;
using CapitalGain.Domain.Settings;
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

    List<TaxResult> result = calculator.ProcessOperations(operations);
    Console.WriteLine(JsonSerializer.Serialize(result));
}

void ConfigureServices(IServiceCollection services)
{
    services.Configure<TaxSettings>(options =>
    {
        options.TaxExemptionThreshold = 20000.00m;
        options.TaxRate = 0.2m;
    });

    services.AddSingleton<ICapitalGainCalculator, CapitalGainCalculator>();
}