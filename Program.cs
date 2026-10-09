using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsuariosApi.Data;
using UsuariosApi.Interfaces;
using UsuariosApi.Repositories;
using UsuariosApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Las validaciones automáticas ([Required], [Range], etc.) también
        // responden HTTP 400 con { "mensaje": "..." }, igual que el resto
        // de errores controlados, para que Android las pueda mostrar.
        options.InvalidModelStateResponseFactory = context =>
        {
            var mensajes = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                    ? "Solicitud no válida."
                    : e.ErrorMessage)
                .Distinct()
                .ToList();

            return new BadRequestObjectResult(new
            {
                mensaje = string.Join(" ", mensajes)
            });
        };
    });

// El nombre debe coincidir con appsettings.json -> ConnectionStrings
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Falta ConnectionStrings:DefaultConnection en appsettings.json");

// XAMPP trae MariaDB; AutoDetect funciona con MariaDB y con MySQL.
// Si la base aún no responde se usa una versión de MariaDB por defecto.
ServerVersion serverVersion;
try
{
    serverVersion = ServerVersion.AutoDetect(connectionString);
}
catch
{
    serverVersion = new MariaDbServerVersion(new Version(10, 4, 32));
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Para la práctica local con el emulador Android
// evitamos redireccionar HTTP hacia HTTPS.
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();
