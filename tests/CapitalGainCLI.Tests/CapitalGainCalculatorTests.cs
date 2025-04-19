using CapitalGain.Application.Services;
using CapitalGain.Domain.Entities;
using CapitalGain.Domain.Enums;
using CapitalGain.Domain.Settings;
using Microsoft.Extensions.Options;
using Xunit;

public class CapitalGainCalculatorTests
{
    [Fact]
    public void Should_Calculate_Tax_Correctly_For_Multiple_Operations()
    {
        // Arrange
        TaxSettings taxSettings = new TaxSettings
        {
            TaxExemptionThreshold = 20000.00m,
            TaxRate = 0.2m
        };
        IOptions<TaxSettings> options = Options.Create(taxSettings);
        CapitalGainCalculator calculator = new CapitalGainCalculator(options);

        List<Operation> operations = new List<Operation>
        {
            new Operation { OperationType = OperationType.Buy, UnitCost = 10.00m, Quantity = 10000 },
            new Operation { OperationType = OperationType.Sell, UnitCost = 20.00m, Quantity = 5000 },
            new Operation { OperationType = OperationType.Sell, UnitCost = 5.00m, Quantity = 5000 }
        };

        // Act
        List<TaxResult> results = calculator.ProcessOperations(operations);

        // Assert
        Assert.Equal(0.0m, results[0].Tax); // Buy = no tax
        Assert.Equal(10000.0m, results[1].Tax); // 20% on R$50,000 profit
        Assert.Equal(0.0m, results[2].Tax); // Loss = no tax
    }

    [Fact]
    public void Should_Subtract_Loss_From_Profit_Before_Calculating_Tax()
    {
        // Arrange
        TaxSettings taxSettings = new TaxSettings
        {
            TaxExemptionThreshold = 20000.00m,
            TaxRate = 0.2m
        };
        IOptions<TaxSettings> options = Options.Create(taxSettings);
        CapitalGainCalculator calculator = new CapitalGainCalculator(options);

        List<Operation> operations = new List<Operation>
        {
            new Operation { OperationType = OperationType.Buy, UnitCost = 10.00m, Quantity = 10000 },
            new Operation { OperationType = OperationType.Sell, UnitCost = 5.00m, Quantity = 5000 },
            new Operation { OperationType = OperationType.Sell, UnitCost = 20.00m, Quantity = 3000 }
        };

        // Act
        List<TaxResult> results = calculator.ProcessOperations(operations);

        // Assert
        Assert.Equal(0.0m, results[0].Tax); // Buy = no tax
        Assert.Equal(0.0m, results[1].Tax); // Loss = no tax
        Assert.Equal(1000.0m, results[2].Tax); // 20% on R$25,000 (profit after subtracting the previous loss of R$25,000)
    }

    [Fact]
    public void Should_Handle_Weighted_Average_Cost_With_No_Loss_Or_Profit()
    {
        // Arrange
        TaxSettings taxSettings = new TaxSettings
        {
            TaxExemptionThreshold = 20000.00m,
            TaxRate = 0.2m
        };
        IOptions<TaxSettings> options = Options.Create(taxSettings);
        CapitalGainCalculator calculator = new CapitalGainCalculator(options);

        List<Operation> operations = new List<Operation>
        {
            new Operation { OperationType = OperationType.Buy, UnitCost = 10.00m, Quantity = 10000 },
            new Operation { OperationType = OperationType.Buy, UnitCost = 25.00m, Quantity = 5000 },
            new Operation { OperationType = OperationType.Sell, UnitCost = 15.00m, Quantity = 10000 }
        };

        // Act
        List<TaxResult> results = calculator.ProcessOperations(operations);

        // Assert
        Assert.Equal(0.0m, results[0].Tax); // Buy = no tax
        Assert.Equal(0.0m, results[1].Tax); // Buy = no tax
        Assert.Equal(0.0m, results[2].Tax); // Sell = no tax (considering the weighted average cost)
    }

    [Fact]
    public void Should_Calculate_Tax_Correctly_Using_Weighted_Average_Cost()
    {
        // Arrange
        TaxSettings taxSettings = new TaxSettings
        {
            TaxExemptionThreshold = 20000.00m,
            TaxRate = 0.2m
        };
        IOptions<TaxSettings> options = Options.Create(taxSettings);
        CapitalGainCalculator calculator = new CapitalGainCalculator(options);

        List<Operation> operations = new List<Operation>
        {
            new Operation { OperationType = OperationType.Buy, UnitCost = 10.00m, Quantity = 10000 },
            new Operation { OperationType = OperationType.Buy, UnitCost = 25.00m, Quantity = 5000 },
            new Operation { OperationType = OperationType.Sell, UnitCost = 15.00m, Quantity = 10000 },
            new Operation { OperationType = OperationType.Sell, UnitCost = 25.00m, Quantity = 5000 }
        };

        // Act
        List<TaxResult> results = calculator.ProcessOperations(operations);

        // Assert
        Assert.Equal(0.0m, results[0].Tax); // Buy = no tax
        Assert.Equal(0.0m, results[1].Tax); // Buy = no tax
        Assert.Equal(0.0m, results[2].Tax); // Sell = no tax (considering the weighted average cost)
        Assert.Equal(10000.0m, results[3].Tax); // Sell = no tax (considering the weighted average cost)
    }

    [Fact]
    public void Should_Calculate_Tax_Correctly_With_Custom_TaxSettings()
    {
        // Arrange
        TaxSettings taxSettings = new TaxSettings
        {
            TaxExemptionThreshold = 10000.00m,
            TaxRate = 0.15m
        };
        IOptions<TaxSettings> options = Options.Create(taxSettings);
        CapitalGainCalculator calculator = new CapitalGainCalculator(options);

        List<Operation> operations = new List<Operation>
    {
        new Operation { OperationType = OperationType.Buy, UnitCost = 10.00m, Quantity = 10000 },
        new Operation { OperationType = OperationType.Sell, UnitCost = 20.00m, Quantity = 5000 },
        new Operation { OperationType = OperationType.Sell, UnitCost = 5.00m, Quantity = 5000 }
    };

        // Act
        List<TaxResult> results = calculator.ProcessOperations(operations);

        // Assert
        Assert.Equal(0.0m, results[0].Tax); // Buy = no tax
        Assert.Equal(7500.0m, results[1].Tax); // 15% on R$50,000 profit (higher than the R$10,000 limit)
        Assert.Equal(0.0m, results[2].Tax); // Sell = no tax (loss)
    }

    [Fact]
    public void Should_Throw_Exception_When_Operations_List_Is_Empty()
    {
        // Arrange
        TaxSettings taxSettings = new TaxSettings { TaxExemptionThreshold = 20000.00m, TaxRate = 0.2m };
        IOptions<TaxSettings> options = Options.Create(taxSettings);
        CapitalGainCalculator calculator = new CapitalGainCalculator(options);

        List<Operation> operations = new List<Operation>();

        // Act
        List<TaxResult> result = calculator.ProcessOperations(operations);

        Console.WriteLine("Result: " + result.Count);

        // Assert
        Assert.Empty(result); // Should return an empty list without throwing an exception
    }

    [Fact]
    public void Should_Throw_Exception_When_Operation_Has_Invalid_Quantity_Or_UnitCost()
    {
        // Arrange
        TaxSettings taxSettings = new TaxSettings { TaxExemptionThreshold = 20000.00m, TaxRate = 0.2m };
        IOptions<TaxSettings> options = Options.Create(taxSettings);
        CapitalGainCalculator calculator = new CapitalGainCalculator(options);

        List<Operation> operations = new List<Operation>
        {
            new Operation { OperationType = OperationType.Buy, UnitCost = -10.00m, Quantity = 100 }, // Invalid UnitCost
            new Operation { OperationType = OperationType.Sell, UnitCost = 20.00m, Quantity = -50 }  // Invalid Quantity
        };

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => calculator.ProcessOperations(operations));
        Assert.Equal("One or more operations contain invalid Quantity or UnitCost.", exception.Message);
    }

    [Fact]
    public void Should_Throw_Exception_When_Sell_Exceeds_Available_Quantity()
    {
        // Arrange
        TaxSettings taxSettings = new TaxSettings { TaxExemptionThreshold = 20000.00m, TaxRate = 0.2m };
        IOptions<TaxSettings> options = Options.Create(taxSettings);
        CapitalGainCalculator calculator = new CapitalGainCalculator(options);

        List<Operation> operations = new List<Operation>
        {
            new Operation { OperationType = OperationType.Buy, UnitCost = 10.00m, Quantity = 100 },
            new Operation { OperationType = OperationType.Sell, UnitCost = 20.00m, Quantity = 150 } // Exceeds available quantity
        };

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => calculator.ProcessOperations(operations));

        // Assert
        Assert.Equal("One or more sell operations exceed available quantity.", exception.Message);
    }
}