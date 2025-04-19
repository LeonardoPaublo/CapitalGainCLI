using System.Text.Json.Serialization;
using CapitalGain.Domain.Enums;

namespace CapitalGain.Domain.Entities;

public class Operation
{
    [JsonPropertyName("operation")]
    public OperationType OperationType { get; set; }

    [JsonPropertyName("unit-cost")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}