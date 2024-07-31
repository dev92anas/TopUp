using TopUp.Application.ExternalServices;
using TopUp.Application.Services.IServices;
using TopUp.Application.Services;
using TopUp.Domain.Interfaces;
using TopUp.Infrastructure.Data;
using TopUp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using TopUpService.Application.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SecurityService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<TopUpDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();
builder.Services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILookupsRepository, LookupsRepository>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<ITopUpTransactionRepository, TopUpTransactionRepository>();
builder.Services.AddScoped<TopUpDbContext, TopUpDbContext>();
builder.Services.AddHttpClient<IExternalBalanceService, ExternalBalanceService>(client =>
{
    client.BaseAddress = new System.Uri("https://api.example.com/");
});

builder.Services.AddHttpContextAccessor();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

// Register the exception handling middleware
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
