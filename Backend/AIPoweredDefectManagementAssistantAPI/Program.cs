using AIPoweredDefectManagementAssistant.Datalayer;
using AIPoweredDefectManagementAssistant.Services.AzureService;
using AIPoweredDefectManagementAssistant.Services.FileService;
using AIPoweredDefectManagementAssistant.Services.OpenAIService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Validate and read connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured in appsettings.json.");
}

// Register an IDbConnection factory (MS SQL)
builder.Services.AddTransient<IDbConnection>(_ => new SqlConnection(connectionString));

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllers(); // <-- Required to avoid the InvalidOperationException
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddScoped<IAzureOpenAIClient, AzureOpenAIClient>();
builder.Services.AddScoped<IAzureServices, AzureServices>();

builder.Services.AddSingleton<ExcelService>();
// Register Azure OpenAI client

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();