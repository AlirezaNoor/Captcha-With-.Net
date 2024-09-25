using StackExchange.Redis;
using test.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICaptchaService, CaptchaService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;

// اضافه کردن اتصال Redis به DI container
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()    // Allow requests from any origin
            .AllowAnyMethod()    // Allow any HTTP method (GET, POST, etc.)
            .AllowAnyHeader();   // Allow any headers
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowAll");
app.Run();