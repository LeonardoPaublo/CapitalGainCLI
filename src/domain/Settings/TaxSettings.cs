namespace CapitalGain.Domain.Settings
{
    public class TaxSettings
    {
        public decimal TaxExemptionThreshold { get; set; } = 20000.00m;
        public decimal TaxRate { get; set; } = 0.2m;
    }
}