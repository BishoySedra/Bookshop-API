using Core.Interfaces;
using DataAccess;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Adding services to the container

// Adding mainContext to the container.
builder.Services.AddDbContext<mainContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adding repositories to the container.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Adding autoMapper to the container.
builder.Services.AddAutoMapper(typeof(Program));

// Adding services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();

// Adding Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
