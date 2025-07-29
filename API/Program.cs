using Core.Interfaces;
using DataAccess;
using DataAccess.Repositories;
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
