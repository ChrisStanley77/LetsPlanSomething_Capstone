using Microsoft.EntityFrameworkCore;

public class AccountDB : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL("server=localhost;database=letsplansomething;user=root;password=abc123!!@");
    }
    public DbSet<Account> Accounts => Set<Account>();

    public AccountDB()
    {

    }
}