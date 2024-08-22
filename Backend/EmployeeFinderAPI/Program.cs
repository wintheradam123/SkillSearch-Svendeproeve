using Shared;
using SkillSearchAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://localhost:4200", "localhost:4200",
                    "http://localhost:7208", "https://localhost:7208")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

//services.AddMemoryCache();


builder.Services.AddDbContext<Context>();
builder.Services.AddScoped<UserController>();
builder.Services.AddScoped<SkillController>();


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

app.UseCors("AllowSpecificOrigin");

app.Run();
