using CapitalGain.Domain.Entities;

namespace CapitalGain.Domain.Services;

public interface ICapitalGainCalculator
{
    List<Dictionary<string, decimal>> ProcessOperations(List<Operation> operations);
}