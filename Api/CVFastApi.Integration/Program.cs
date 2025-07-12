using System.Reflection;
using CVFastServices.Data;
using CVFastServices.Repositories;
using CVFastServices.Repositories.Interfaces;
using CVFastServices.Services;
using CVFastServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger específico para integradores
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "CVFast Integration API",
        Description = "API pública para consulta de currículos por sistemas integradores",
        Contact = new OpenApiContact
        {
            Name = "CVFast Support",
            Email = "support@cvfast.com.br"
        }
    });

    // Incluir comentários XML na documentação do Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configuração do banco de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração dos repositórios
builder.Services.AddScoped<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddScoped<IShortLinkRepository, ShortLinkRepository>();
builder.Services.AddScoped<IShortLinkService, ShortLinkService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Configuração do CORS para integradores
builder.Services.AddCors(options =>
{
    options.AddPolicy("IntegrationPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Swagger sempre habilitado para integradores
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CVFast Integration API v1");
    options.RoutePrefix = string.Empty; // Para servir a UI do Swagger na raiz
    options.DocumentTitle = "CVFast Integration API";
});

app.UseCors("IntegrationPolicy");

app.MapControllers();

app.Run();
