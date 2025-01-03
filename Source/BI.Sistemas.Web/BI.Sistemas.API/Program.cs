using BI.Sistemas.API.Interfaces;
using BI.Sistemas.API.Repository;
using BI.Sistemas.API.Service;
using BI.Sistemas.Context;


var builder = WebApplication.CreateBuilder(args);

// Adicionar arquivos de configuração
builder.Configuration.AddJsonFile("econometrosettings.json");

// Adicionar serviços ao contêiner
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            // Configura a política CORS
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddEntityFrameworkSqlServer()
    .AddDbContext<BISistemasContext>();

builder.Services.AddScoped<IColaboradorSLAService, ColaboradorSLAService>();
builder.Services.AddScoped<IColaboradorService, ColaboradorService>();
builder.Services.AddScoped<IEconometroService, EconometroService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();

var app = builder.Build();


// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();
