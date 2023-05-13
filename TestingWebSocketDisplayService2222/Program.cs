using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using TestingWebSocketServiceDisplay2222.Hubs;
using TestingWebSocketServiceDisplay2222.Models;
using TestingWebSocketServiceDisplay.Services;
using TestingWebSocketServiceDisplay2222.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // Add IConfiguration to the service container
builder.Services.AddSingleton(builder.Configuration.GetValue<string>("WebSocketUrl")); // Register the WebSocketUrl value as a service
builder.Services.AddSingleton<ConcurrentDictionary<string, Event>>();

// Register the background task as a hosted service
builder.Services.AddHostedService<MyBackgroundTask>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseCors();
app.MapHub<MyHub>("/MyHub");
app.Run();