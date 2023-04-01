using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SavageOrcs.BusinessObjects;
using SavageOrcs.DbContext;
using SavageOrcs.Repositories;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using SavageOrcs.Web.Resources.Classes;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SavageOrcsDbContextConnection") ?? throw new InvalidOperationException("Connection string 'SavageOrcsDbContextConnection' not found.");




builder.Services.AddDbContext<SavageOrcsDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SavageOrcsDbContext>();

// Add services to the container.
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IAreaService, AreaService>();
builder.Services.AddTransient<IMapService, MapService>();
builder.Services.AddTransient<IMarkService, MarkService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<ICuratorService, CuratorService>();
builder.Services.AddTransient<IClusterService, ClusterService>();
builder.Services.AddTransient<IHelperService, HelperService>();
builder.Services.AddTransient<ITextService, TextService>();
builder.Services.AddTransient<IKeyWordService, KeyWordService>();



builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var asseblyName = new AssemblyName(typeof(MainResource).GetTypeInfo().Assembly.FullName);
            return factory.Create("MainResource", asseblyName.Name);
        };
    });

builder.Services.Configure<RequestLocalizationOptions>(
    options =>
    {
        var supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("uk-UA")
        };
        options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/ServerError");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var options = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(options.Value);

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Map}/{action=Main}");
app.MapControllerRoute(
    name: "Error",
    pattern: "{*val}",
    defaults: new { controller = "Error", action = "Error"});

app.MapRazorPages();

app.Run();
