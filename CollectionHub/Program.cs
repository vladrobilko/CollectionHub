using CollectionHub.Services.Interfaces;
using CollectionHub.Services;
using Microsoft.EntityFrameworkCore;
using CollectionHub.DataManagement;
using CollectionHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Algolia.Search.Clients;
using CollectionHub.Domain;
using CollectionHub.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews();

builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection("Azure"));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IAlgoliaIntegration, AlgoliaIntegration>();
builder.Services.AddScoped<ICollectionFieldNameUpdater, CollectionFieldNameUpdater>();

builder.Services.AddSingleton<ISearchClient>(new SearchClient(
    builder.Configuration["AlgoliaApplicationId"], builder.Configuration["AlgoliaApiKey"]));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));

builder.Services.AddIdentity<UserDb, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 0;
});

builder.Services.AddRazorPages()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var app = builder.Build();

var cultures = new List<CultureInfo> {
    new CultureInfo("en"),
    new CultureInfo("ka")
};

app.UseRequestLocalization(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
