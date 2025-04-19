using CapitalGain.Domain.Entities;
using CapitalGain.Domain.Enums;
using CapitalGain.Application.Extensions;
using CapitalGain.Domain.Services;
using CapitalGain.Domain.Settings;
using Microsoft.Extensions.Options;

namespace CapitalGain.Application.Services;

public class CapitalGainCalculator : ICapitalGainCalculator
{
    private readonly TaxSettings _settings;

    public CapitalGainCalculator(IOptions<TaxSettings> options)
    {
        _settings = options.Value;
    }

    public List<TaxResult> ProcessOperations(List<Operation> operations)
    {
        ValidateOperations(operations);

        List<TaxResult> result = new();

        decimal averageCost = 0;
        int totalQuantity = 0;
        decimal accumulatedLoss = 0;

        foreach (Operation op in operations)
        {
            if (op.OperationType == OperationType.Buy)
            {
                decimal totalCost = averageCost * totalQuantity + op.UnitCost * op.Quantity;
                totalQuantity += op.Quantity;
                averageCost = (totalQuantity == 0 ? 0 : totalCost) / totalQuantity;

                result.Add(new TaxResult { Tax = 0.0m });
            }
            else if (op.OperationType == OperationType.Sell)
            {
                decimal totalSellValue = op.UnitCost * op.Quantity;
                decimal totalCostValue = averageCost * op.Quantity;
                decimal profit = totalSellValue - totalCostValue;

                decimal tax = 0.0m;

                if (profit < 0)
                {
                    accumulatedLoss += Math.Abs(profit);
                }
                else
                {
                    decimal adjustedProfit = profit;

                    if (accumulatedLoss > 0)
                    {
                        decimal deduction = Math.Min(accumulatedLoss, adjustedProfit);
                        adjustedProfit -= deduction;
                        accumulatedLoss -= deduction;
                    }

                    if (totalSellValue > _settings.TaxExemptionThreshold && adjustedProfit > 0)
                    {
                        tax = adjustedProfit * _settings.TaxRate;
                    }
                }

                totalQuantity -= op.Quantity;
                result.Add(new TaxResult { Tax = Math.Round(tax, 2) });
            }
        }

        return result;
    }

    private void ValidateOperations(List<Operation> operations)
    {
        if (operations.IsEmpty())
        {
            return;
        }

        int totalQuantity = 0;

        foreach (var op in operations)
        {
            if (!op.IsValid())
            {
                throw new ArgumentException("One or more operations contain invalid Quantity or UnitCost.");
            }

            if (op.IsSellExceedingAvailableQuantity(totalQuantity))
            {
                throw new InvalidOperationException("One or more sell operations exceed available quantity.");
            }

            if (op.OperationType == OperationType.Buy)
            {
                totalQuantity += op.Quantity;
            }
            else if (op.OperationType == OperationType.Sell)
            {
                totalQuantity -= op.Quantity;
            }
        }
    }
}