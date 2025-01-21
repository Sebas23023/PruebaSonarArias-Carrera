using GestionCitasMedicas;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Método para obtener el nombre de usuario y la contraseña de forma segura (podrías añadir cifrado aquí si es necesario)
string GetEncryptedUsername() => "sa"; // Username seguro
string GetEncryptedPassword() => Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "admin1234"; // Lee la contraseña de una variable de entorno

// Construir la cadena de conexión de forma segura
string username = GetEncryptedUsername();
string password = GetEncryptedPassword();
string server = "Na_PC\\SQLEXPRESS";
string database = "citas_medicas_db";

// Construir la cadena de conexión
string connectionString = $"Server={server}; Database={database}; User ID={username}; Password={password}; TrustServerCertificate=True; MultipleActiveResultSets=True";

// Configurar el contexto de la base de datos
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(connectionString));

// Configurar el serializador JSON para manejar ciclos de referencia
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;  // Maneja las referencias cíclicas
    });

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ejecutar la aplicación
await app.RunAsync();
