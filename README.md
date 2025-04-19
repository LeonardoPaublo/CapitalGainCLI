# CapitalGainCLI

CapitalGainCLI is a command-line application designed to calculate capital gains taxes based on a series of buy and sell operations. It uses a weighted average cost method to determine profits and applies tax rules based on configurable settings.

---

## Features

- **Weighted Average Cost Calculation**: Automatically calculates the average cost of assets based on buy operations.
- **Tax Calculation**: Applies tax rates and exemptions based on configurable thresholds.
- **Loss Deduction**: Subtracts accumulated losses from profits before calculating taxes.
- **JSON Input/Output**: Accepts operations in JSON format and outputs tax results in JSON format.

---

## Project Structure
```
CapitalGainCLI/
├── src/
│   ├── application/
│   │   ├── Extensions/
│   │   │   └── OperationExtensions.cs
│   │   └── Services/
│   │       └── CapitalGainCalculator.cs
│   ├── domain/
│   │       ├── Entities
│   │       │   ├── Operations.cs
│   │       │   └── TaxResult.cs
│   │       ├── Enums
│   │       │   └── OperationType.cs
│   │       ├── Services
│   │       │   └── ICapitalGainCalculator.cs
│   │       └── Settings
│   │           └── TaxSettings.cs
│   ├── CapitalGainCLI.csproj
│   ├── Program.cs
├── tests/
│   └── CapitalGainCLI.Tests/
│       ├── CapitalGainCLI.Tests.csproj
│       └── CapitalGainCalculatorTests.cs
├── .gitignore
├── CapitalGainCLI.sln
└── README.md
```

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

---

### Running the Application

1. Clone the repository:
   ```bash
   git clone https://github.com/LeonardoPaublo/CapitalGainCLI.git
   cd CapitalGainCLI
   ```
2. Run the application:
   ```bash
   dotnet run --project src/CapitalGainCLI.csproj
   ```
3. Provide JSON input for operations via the console. Example:
   ```JSON
   [{ "operation": "buy", "unit-cost": 10.00, "quantity": 10000 },{ "operation": "sell", "unit-cost": 20.00, "quantity": 5000 }]
   ```
4. The application will output the tax results in JSON format:
   ```JSON
   [{ "tax": 0.0 },{ "tax": 10000.00 }]
   ```

---

### Running Tests
1. Navigate to the root directory of the project:
2. Run the tests:
   ```bash
   dotnet test
   ```

---

## Configuration

The application uses the ```TaxSettings``` class to configure tax rules:
- *TaxExemptionThreshold*: The minimum profit required before taxes are applied.
- *TaxRate*: The percentage of profit to be taxed.
You can modify these settings in the ```ConfigureServices``` method in ```Program.cs``` or directly in the test cases.
