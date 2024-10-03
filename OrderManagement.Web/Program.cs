using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Web.Hub;
using OrderManagement.Web.Services;
using OrderManagement.Web.Services.IServices;
using OrderManagement.Web.Utilities;
using OrderManagement.Web.Utilities.FluentValidator;


var builder = WebApplication.CreateBuilder(args);
//SignalR Service
builder.Services.AddSignalR();

//Populate the OrderAPIBase Urls stored in ProgramCs
SD.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"];

// Add services to the container.
//Configure the Http Client
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IOrderService,OrderService>();

//Service lifetime
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddControllersWithViews();

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<OrderValidator>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Disable automatic ModelState validation
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
//SignalR Mapping
app.MapHub<OrderHub>("/orderHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
