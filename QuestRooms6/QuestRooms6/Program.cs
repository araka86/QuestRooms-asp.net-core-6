using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestRooms6.Initializer;
using QuestRooms6_DataAccess.Data;
using QuestRooms6_Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//entity register
builder.Services.AddDbContext<QuestRoomsContextDb>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("QuestRoomDefault")));


//identity regiser
builder.Services.AddIdentity<AplicationUser, IdentityRole>()
                    .AddDefaultTokenProviders()
                    .AddDefaultUI()
                    .AddEntityFrameworkStores<QuestRoomsContextDb>();

//identity Path
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/Account/Login";
});



builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddControllersWithViews();

//session
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();


//Add service to conveyer (initializing)
var dbInitializer = app.Services.CreateScope();
var ini = dbInitializer.ServiceProvider.GetRequiredService<IDbInitializer>();
ini.Initialize();


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

//add session
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
