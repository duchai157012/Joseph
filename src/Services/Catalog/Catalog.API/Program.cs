using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Layer
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Catalog.Application.Features.Products.Commands.CreateProduct.CreateProductCommand).Assembly));

// Infrastructure Layer
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogDbContext>());

// JWT Auth Config
var key = Encoding.ASCII.GetBytes("ThisIsASecretKeyForJwtTokenGenerationYOUR_SECRET_KEY_HERE");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Auto-migration for demo purposes
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    try 
    {
         context.Database.EnsureCreated();
    }
    catch(Exception ex)
    {
        // Log error
        Console.WriteLine(ex.Message);
    }
}

app.Run();
