using Microsoft.EntityFrameworkCore;
using Payments.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseKestrel();
builder.Services.AddDbContext<PaymentsContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentsContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PaymentsContext>();
    context.Database.EnsureCreated();
}
    
var waitHandle = new AutoResetEvent(false);
ThreadPool.RegisterWaitForSingleObject(
  waitHandle, 
  // Method to execute
  (state, timeout) => 
  {
    using (var scope = app.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<PaymentsContext>();
        context.Database.ExecuteSqlRaw("DELETE FROM payments");
        
        var logger = services.GetService<ILogger<Program>>();
        logger.LogInformation("Clearing database...");
    }
  }, 
  // optional state object to pass to the method
  null, 
  // Execute the method after 5 seconds
  TimeSpan.FromMinutes(10), 
  // Set this to false to execute it repeatedly every 5 seconds
  false
);

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
