using System.ComponentModel.DataAnnotations;

namespace BasicBankingAPI.DTOs;

public class CreateAccountDto : IValidatableObject
{
    [Required]
    [StringLength(100)]
    public string AccountHolderName { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string AccountNumber { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal Balance { get; set; }

    [Required]
    [RegularExpression(
        "^(Savings|Current)$",
        ErrorMessage = "AccountType must be Savings or Current.")]
    public string AccountType { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (Balance != decimal.Round(Balance, 2))
        {
            yield return new ValidationResult(
                "Balance must have at most two decimal places.",
                new[] { nameof(Balance) });
        }
    }
}