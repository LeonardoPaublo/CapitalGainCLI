using CapitalGain.Domain.Entities;
using CapitalGain.Domain.Enums;

namespace CapitalGain.Application.Extensions;

public static class OperationExtensions
{
    public static bool IsValid(this Operation operation)
    {
        return operation.Quantity > 0 && operation.UnitCost >= 0;
    }

    public static bool IsSellExceedingAvailableQuantity(this Operation operation, int totalQuantity)
    {
        return operation.OperationType == OperationType.Sell && operation.Quantity > totalQuantity;
    }

    public static bool IsEmpty(this List<Operation> operations)
    {
        return operations == null || operations.Count == 0;
    }
}