using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ECommerce.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("defaultConnection")
    ));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
//builder.Services.AddIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();


//options => options.SignIn.RequireConfirmedAccount = true this part checks the email is confirmed or not, if we remove that even when the email is not confirmed the user would be able to login

//AddEntityFrameworkStores<ApplicationDbContext>();-> this is telling the DB which has so many tables added related to Accounts, Users etc. while adding identity that those tables is binded and 
//modified by ApplicationDbContext

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

//ConfigureApplicationCookie allows you to customize the behavior of authentication cookie, such as expiration path, logout path, access denied path.
//This should be called after AddIdentity

builder.Services.AddRazorPages();  //This service will also consider Razor pages if any in the application

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

builder.Services.AddScoped<IEmailSender,EmailSender>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});


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
app.UseAuthentication();  //this middleware should be added before Authorization because first we need to confirm the user is authenticated to view particular page before entering username and password
app.UseAuthorization();
app.MapRazorPages();        //Used to map the addrazorpages service

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
