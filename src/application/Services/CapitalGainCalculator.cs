using CapitalGain.Domain.Entities;
using CapitalGain.Domain.Enums;
using CapitalGain.Domain.Services;

namespace CapitalGain.Application.Services;

public class CapitalGainCalculator : ICapitalGainCalculator
{
    private decimal averageCost = 0;
    private int totalQuantity = 0;
    private decimal accumulatedLoss = 0;

    public List<Dictionary<string, decimal>> ProcessOperations(List<Operation> operations)
    {
        var result = new List<Dictionary<string, decimal>>();

        foreach (var op in operations)
        {
            if (op.OperationType == OperationType.Buy)
            {
                decimal totalCost = averageCost * totalQuantity + op.UnitCost * op.Quantity;
                totalQuantity += op.Quantity;
                averageCost = totalQuantity == 0 ? 0 : totalCost / totalQuantity;

                result.Add(new Dictionary<string, decimal> { { "tax", 0.0m } });
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

                    if (totalSellValue > 20000.00m && adjustedProfit > 0)
                    {
                        tax = adjustedProfit * 0.2m;
                    }
                }

                totalQuantity -= op.Quantity;
                result.Add(new Dictionary<string, decimal> { { "tax", Math.Round(tax, 2) } });
            }
        }

        return result;
    }
}