using System.ComponentModel.DataAnnotations;

namespace BasicBankingAPI.DTOs;

public class TransactionDto : IValidatableObject
{
    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99",
        ErrorMessage = "Amount must be positive and within the supported range.")]
    public decimal Amount { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (Amount != decimal.Round(Amount, 2))
        {
            yield return new ValidationResult(
                "Amount must have at most two decimal places.",
                new[] { nameof(Amount) });
        }
    }
}