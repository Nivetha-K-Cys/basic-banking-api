using System.ComponentModel.DataAnnotations;

namespace BasicBankingAPI.Models;

public class BankAccount
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string AccountHolderName { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string AccountNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    [Required]
    [StringLength(10)]
    public string AccountType { get; set; } = string.Empty;
}