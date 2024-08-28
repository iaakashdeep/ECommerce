using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ECommerce.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using ECommerce.DataAccess.DBInitializer;

//This will set up the Web Host and Generic Host in background
//we can configure services, logging using builder.Host
var builder = WebApplication.CreateBuilder(args);

#region Configuring services through generic Host
builder.Host.ConfigureServices((hostcontext, services) =>
{
    // Add services to the container.

    services.AddControllersWithViews();
    //services.AddControllersWithViews(options=>
    //{
    //    options.Filters.Add<BaseExceptionController>();
    //});



    //services.AddHostedService<LoggingUtility>();

    #region Connection String

    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    hostcontext.Configuration.GetConnectionString("defaultConnection")
    ));

    #endregion

    #region Stripe for Payment Gateway Service

    //This service will automatically inject the stripe values from appsettings to properties in StripeSetting class
    services.Configure<StripeSettings>(hostcontext.Configuration.GetSection("Stripe"));

    #endregion

    #region Identity using .NET
    services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

    services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = $"/Identity/Account/Login";
        options.LogoutPath = $"/Identity/Account/Logout";
        options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    });

    #endregion

    #region Session Logic
    services.AddDistributedMemoryCache();
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(100);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
    #endregion

    #region Facebook Authentication using OAuth
    services.AddAuthentication().AddFacebook(options =>
    {
        options.AppId = "886007550040286";
        options.AppSecret = "cbcf41c7242f292b55fef3f92e8e10fa";
    });
    #endregion

    #region Service for Seeding DB for first time if roles and user hasn't been created

    services.AddScoped<IDBInitializer, DBInitializer>();
    #endregion

    services.AddRazorPages();

    services.AddScoped<IUnitOfWork, UnitOfWork>();

    services.AddScoped<IEmailSender, EmailSender>();

    services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 104857600; // 100 MB
    });
}
);
#endregion

#region Configure Logging through Generic Host

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
}
);

#endregion

//The above methods use Generic Host explicitly for configuring services, but you can directly use builder.Services to do that as we did in other modules





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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();        // We need to set the API key for stripe
app.UseRouting();
app.UseAuthentication();  
app.UseAuthorization();
app.UseSession();           //To use sessions in request pipeline
SeedDatabase();             //For seeding DB first time using DB initializer
app.MapRazorPages();        

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


//
void SeedDatabase()
{
    using(var scope=app.Services.CreateScope())
    {
        var dbInitializer=scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        dbInitializer.Initialize();
    }
}
