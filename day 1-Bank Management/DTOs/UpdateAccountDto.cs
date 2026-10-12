using System.ComponentModel.DataAnnotations;

namespace BasicBankingAPI.DTOs;

public class UpdateAccountDto
{
    [Required]
    [StringLength(100)]
    public string AccountHolderName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        "^(Savings|Current)$",
        ErrorMessage = "AccountType must be Savings or Current.")]
    public string AccountType { get; set; } = string.Empty;
}