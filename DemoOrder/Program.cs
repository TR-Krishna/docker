using DemoOrder.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Meter Management Service",
        Version = "v1"
    });

});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var retries = 10;
    while (retries > 0)
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
            Console.WriteLine("Database migration successful.");
            break;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration failed. Retries left: {retries - 1}. Error: {ex.Message}");
            retries--;
            Thread.Sleep(5000); // Wait for 5 seconds before retrying
        }
    }
}
//app.UseSwagger();
//app.UseSwaggerUI();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeterManagementService V1");
    c.RoutePrefix = "swagger"; // access at /swagger instead of /api/order/swagger
});
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();