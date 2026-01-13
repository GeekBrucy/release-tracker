using Microsoft.EntityFrameworkCore;
using ReleaseTracker.Web.Data;
using ReleaseTracker.Web.Models;
using ReleaseTracker.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HttpContextAccessor for user service
builder.Services.AddHttpContextAccessor();

// Register UserService
builder.Services.AddScoped<IUserService, UserService>();

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ReleaseTrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ReleaseOptions from appsettings.json
builder.Services.Configure<ReleaseOptions>(
    builder.Configuration.GetSection("ReleaseOptions"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
