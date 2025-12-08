using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Application.Interfaces;
using Order.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Layer
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Order.Application.Features.Orders.Commands.CreateOrder.CreateOrderCommand).Assembly));

// Infrastructure Layer (EF Core)
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderDbContext>(provider => provider.GetRequiredService<OrderDbContext>());

// MassTransit (RabbitMQ)
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Auto-migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    try 
    {
         context.Database.EnsureCreated();
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

app.Run();
