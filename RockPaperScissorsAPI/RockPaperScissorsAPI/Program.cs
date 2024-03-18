using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.GrpcServices;






var builder = WebApplication.CreateBuilder(args);

// конфигурация Kestrel для поддержки HTTP/2 протокола (для gRPC)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5132, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// добавление сервисов
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // подключение к БД
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization(); 

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<GameServiceImplementation>(); // маппинг gRPC сервиса
});

app.Run();

