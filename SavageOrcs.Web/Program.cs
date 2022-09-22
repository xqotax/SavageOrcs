using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SavageOrcs.DbContext;
using SavageOrcs.Repositories;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SavageOrcsDbContextConnection") ?? throw new InvalidOperationException("Connection string 'SavageOrcsDbContextConnection' not found.");




builder.Services.AddDbContext<SavageOrcsDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SavageOrcsDbContext>();

// Add services to the container.
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IEmailService, EmailService>();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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
    pattern: "{controller=Map}/{action=Main}");
app.MapRazorPages();

app.Run();
