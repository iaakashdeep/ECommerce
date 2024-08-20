using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ECommerce.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;

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

    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    hostcontext.Configuration.GetConnectionString("defaultConnection")
    ));

    services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

    services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = $"/Identity/Account/Login";
        options.LogoutPath = $"/Identity/Account/Logout";
        options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    });


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

app.UseRouting();
app.UseAuthentication();  
app.UseAuthorization();
app.MapRazorPages();        

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
