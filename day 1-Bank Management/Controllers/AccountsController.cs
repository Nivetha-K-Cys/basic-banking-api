using BasicBankingAPI.Data;
using BasicBankingAPI.DTOs;
using BasicBankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BasicBankingAPI.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly AppDbContext _context;

    private const decimal MaximumBalance = 9999999999999999.99m;

    public AccountsController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/accounts
    [HttpPost]
    public async Task<ActionResult<BankAccount>> CreateAccount(
        CreateAccountDto dto)
    {
        string accountNumber = dto.AccountNumber.Trim();

        bool numberExists = await _context.Accounts
            .AnyAsync(account =>
                account.AccountNumber == accountNumber);

        if (numberExists)
        {
            return Conflict(new
            {
                message = "Account number already exists."
            });
        }

        var account = new BankAccount
        {
            AccountHolderName = dto.AccountHolderName.Trim(),
            AccountNumber = accountNumber,
            Balance = dto.Balance,
            AccountType = dto.AccountType
        };

        _context.Accounts.Add(account);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlException &&
                  (sqlException.Number == 2601 ||
                   sqlException.Number == 2627))
        {
            return Conflict(new
            {
                message = "Account number already exists."
            });
        }

        return CreatedAtAction(
            nameof(GetAccountById),
            new { id = account.Id },
            account);
    }

    // GET /api/accounts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BankAccount>>> GetAccounts()
    {
        var accounts = await _context.Accounts
            .AsNoTracking()
            .OrderBy(account => account.Id)
            .ToListAsync();

        return Ok(accounts);
    }

    // GET /api/accounts/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BankAccount>> GetAccountById(int id)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (account == null)
        {
            return NotFound(new { message = "Account not found." });
        }

        return Ok(account);
    }

    // PUT /api/accounts/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BankAccount>> UpdateAccount(
        int id,
        UpdateAccountDto dto)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (account == null)
        {
            return NotFound(new { message = "Account not found." });
        }

        account.AccountHolderName = dto.AccountHolderName.Trim();
        account.AccountType = dto.AccountType;

        await _context.SaveChangesAsync();

        return Ok(account);
    }

    // DELETE /api/accounts/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (account == null)
        {
            return NotFound(new { message = "Account not found." });
        }

        // Exercise rule: any sample account may be deleted.
        _context.Accounts.Remove(account);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST /api/accounts/1/deposit
    [HttpPost("{id:int}/deposit")]
    public async Task<ActionResult<BankAccount>> Deposit(
        int id,
        TransactionDto dto)
    {
        int updatedRows = await _context.Accounts
            .Where(account =>
                account.Id == id &&
                account.Balance <= MaximumBalance - dto.Amount)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    account => account.Balance,
                    account => account.Balance + dto.Amount));

        if (updatedRows == 0)
        {
            bool accountExists = await _context.Accounts
                .AnyAsync(account => account.Id == id);

            if (!accountExists)
            {
                return NotFound(new { message = "Account not found." });
            }

            return BadRequest(new
            {
                message = "Deposit exceeds the supported balance limit."
            });
        }

        var account = await _context.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(account => account.Id == id);

        if (account == null)
        {
            return NotFound(new { message = "Account not found." });
        }

        return Ok(account);
    }

    // POST /api/accounts/1/withdraw
    [HttpPost("{id:int}/withdraw")]
    public async Task<ActionResult<BankAccount>> Withdraw(
        int id,
        TransactionDto dto)
    {
        int updatedRows = await _context.Accounts
            .Where(account =>
                account.Id == id &&
                account.Balance >= dto.Amount)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    account => account.Balance,
                    account => account.Balance - dto.Amount));

        if (updatedRows == 0)
        {
            bool accountExists = await _context.Accounts
                .AnyAsync(account => account.Id == id);

            if (!accountExists)
            {
                return NotFound(new { message = "Account not found." });
            }

            return BadRequest(new { message = "Insufficient balance." });
        }

        var account = await _context.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(account => account.Id == id);

        if (account == null)
        {
            return NotFound(new { message = "Account not found." });
        }

        return Ok(account);
    }
}