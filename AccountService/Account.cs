public class Account
{
    public int Id { get; set; }
    //Firstname field
    public string Firstname {get; set;} = null!;
    //Lastname field
    public string Lastname {get; set;} = null!;
    //Email field
    public string Email {get; set;} = null!;
    //Username field
    public string Username {get; set;} = null!;
    //Password field
    public string Password {get; set;} = null!;

    public Account(string Firstname, string Lastname, string Email, string Username, string Password)
    {
        this.Firstname = Firstname;
        this.Lastname = Lastname;
        this.Email = Email;
        this.Username = Username;
        this.Password = Password;
    }
}