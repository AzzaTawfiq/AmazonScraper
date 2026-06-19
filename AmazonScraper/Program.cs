using AmazonScraper.Application.Services;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Shared.Common;
using AmazonScraper.Api.Extensions;
using AmazonScraper.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Infrastructure
builder.Services.AddInfrastructureServices();

//Application
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();

builder.Services.AddOptions<AmazonSelectorOptions>()
    .BindConfiguration("AmazonSelectors");

builder.Services.AddOptions<AmazonURL>()
    .BindConfiguration("AmazonURL");

// 1. Define a unique string for your policy name
string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 2. Register the CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Register PostgreSQL distributed cache with table creation
builder.Services.AddDistributedPostgresCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("PostgresCacheConnection");
    options.SchemaName = "product_cache_db";
    options.TableName = "ProductCacheEntries";
    options.CreateIfNotExists = true;
    options.UseWAL = false; // Optimizes performance
});

// Register the IExceptionHandler and the native ProblemDetails infrastructure
builder.Services.AddExceptionHandler<ScraperExceptionHandler>();
builder.Services.AddProblemDetails(); // Generates metadata for structural error support

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

// 3. Enable the CORS middleware right before Authentication/Authorization
app.UseRouting();

app.UseCors(myAllowSpecificOrigins);

app.UseAuthorization();


app.MapControllers();

app.Run();
