using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.OrderAPI.Data;
using OrderManagement.OrderAPI.Models;
using OrderManagement.OrderAPI.Services;
using OrderManagement.OrderAPI.Services.IServices;
using OrderManagement.OrderAPI.Utilities.FluentValidator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Auto Mapper Services configuration
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper); //Lifetime of the service
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Add Hangfire services.
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<OrderValidator>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Disable automatic ModelState validation
});

builder.Services.AddHttpClient<ICurrencyService,CurrencyService>();

builder.Services.AddScoped<IOrderProcessingService,OrderProcessingService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Hangfire Dashboard for monitoring.
app.UseHangfireDashboard();
// Schedule background jobs
RecurringJob.AddOrUpdate<IOrderProcessingService>(
    "process-pending-orders",
    service => service.ProcessPendingOrdersAsync(),
    Cron.MinuteInterval(5)
);
RecurringJob.AddOrUpdate<IOrderProcessingService>(
        "process-completed-orders",
        service => service.ProcessCompletedOrdersAsync(),
        Cron.MinuteInterval(5)
    );

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
