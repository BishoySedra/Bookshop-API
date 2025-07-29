using Core.Interfaces;
using DataAccess;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region Middleware Setup

// Adding mainContext to the container.
builder.Services.AddDbContext<mainContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adding repositories to the container.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Adding autoMapper to the container.
builder.Services.AddAutoMapper(typeof(Program));

// Adding services to the container.
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default60", new CacheProfile
    {
        Duration = 60, // Cache duration in seconds
        Location = ResponseCacheLocation.Client, // Cache location private or Any for public
        NoStore = false
    });

    options.CacheProfiles.Add("NoCache", new CacheProfile
    {
        NoStore = true,
        Location = ResponseCacheLocation.None
    });
}).AddNewtonsoftJson();

// Adding caching services to the container.
builder.Services.AddResponseCaching(); // Inside builder.Services

// Adding memory cache services to the container.
builder.Services.AddMemoryCache();

// Adding Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Adding API versioning support
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;

    // Use URL versioning: api/v1/controller
    options.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.UrlSegmentApiVersionReader();
});

#endregion


#region Middleware pipeline

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseResponseCaching(); // Enable response caching

app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion