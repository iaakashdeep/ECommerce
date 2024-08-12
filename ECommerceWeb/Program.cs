using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("defaultConnection")
    ));

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

//If we take Product Create view example, the total count of products will increase only once because when we hit the Post request for creating a product a new HTTP request will be generated and for this new HTTP request a new
//instance will be provided so it will increase the count, but the create view is also demanding a instance of Iunitofwork service but this resides in the same scope of create acttion method, means while posting the data for
//create the view will also render at the same time, so these 2 requests are in same scope so, after one click of creation it will ot increase the count

//builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();

//If we take Product Create view example, the total count of products will increase every time we issue a new request whether it is create or reload of page, because Singleton will provide single instance of the service
//thorugh out the app lifecycle and that instance will be used for all HTTP requests.

//builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

//If we take Product Create view example, the total count of products will not increase after hitting the create button because here a count will increase when we hit Create but again a new request comes because this will not consider 
//Create action method request and Create View request in the same scope and thasts why in Transient irrespective of the scope every time a new request comes this will give new service instance

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

//area=Customer we defined because the main index page lies inside Customer area
