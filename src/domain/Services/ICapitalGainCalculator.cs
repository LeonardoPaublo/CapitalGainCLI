using CapitalGain.Domain.Entities;

namespace CapitalGain.Domain.Services;

public interface ICapitalGainCalculator
{
    List<TaxResult> ProcessOperations(List<Operation> operations);
}