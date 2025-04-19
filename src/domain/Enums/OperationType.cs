using System.Text.Json.Serialization;

namespace CapitalGain.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OperationType
{
    Buy,
    Sell
}