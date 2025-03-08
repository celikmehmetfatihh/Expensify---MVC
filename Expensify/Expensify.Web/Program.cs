using Expensify.Web.Data;
using Expensify.Web.Models;
using Expensify.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// General logger for console and debug
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Debug()
.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Hour,
                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ExpensifyDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("ExpensifyDbConnectionString"));
});

// Add Identity Services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
	AddEntityFrameworkStores<ExpensifyDbContext>()
	.AddDefaultTokenProviders();


builder.Services.AddScoped<IExpenseFormRepository, ExpenseFormRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
