using GenesisApi.Interfaces;
using GenesisApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddScoped<ITableParserService, TableParserService>();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

WebApplication webApp = builder.Build();

webApp.UseHttpsRedirection();
webApp.UseAuthorization();
webApp.MapControllers();

webApp.UseSwagger();
webApp.UseSwaggerUI();

webApp.UseCors("AllowAll");

webApp.Run();
