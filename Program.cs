using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestauranteApi.DataBase;
using RestauranteApi.Middleware;
using RestauranteApi.Service.Implementations;
using RestauranteApi.Service.Interfaces;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

if (builder.Environment.IsDevelopment())
{
    var keysDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys"));
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(keysDirectory);
}

// SQLite
builder.Services.AddDbContext<RestauranteApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Jwt:Key debe configurarse mediante variables de entorno o user-secrets.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
                "http://localhost:3000",   // React / Next.js
                "http://localhost:4200",   // Angular
                "http://localhost:5173",   // Vite / React
                "http://localhost:8080"    // Vue
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// Servicios de negocio
builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ITurnService, TurnService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<ILockTableService, LockTableService>();
builder.Services.AddScoped<IWaitingListService, WaitingListService>();

var app = builder.Build();

// Crear DB SQLite y aplicar seeds si no existe
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RestauranteApiDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();
app.Run();
