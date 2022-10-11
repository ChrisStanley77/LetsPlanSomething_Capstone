using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<AccountDB>(opt => opt.UseMySQL(builder.Configuration.GetConnectionString("users")));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();
app.MapControllers();
app.UseCors();

app.MapGet("/", () => "Hello World!");

app.Run();
