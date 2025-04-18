using System.Text.Json.Serialization;

namespace CapitalGain.Domain.Entities;

public class TaxResult
{
    [JsonPropertyName("tax")]
    public decimal Tax { get; set; }
}