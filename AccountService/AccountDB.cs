using Microsoft.EntityFrameworkCore;

public class AccountDB : DbContext
{
    public AccountDB(DbContextOptions<AccountDB> options) : base(options) { }
    public DbSet<Account> Accounts => Set<Account>();

    public AccountDB()
    {

    }
}