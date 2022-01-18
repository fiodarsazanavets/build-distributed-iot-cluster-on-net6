global using DeviceManagementApp.Server;
global using DeviceManagementApp.Server.Hubs;
global using DeviceManagementApp.Server.Models;
global using DeviceManagementApp.Shared;
global using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IEventSchedule, EventSchedule>();
builder.Services.AddSingleton<LocationMapper>();
builder.Services.AddSingleton<IAudioFileManager, AudioFileManager>();
builder.Services.AddHostedService<AudioEventScheduler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapHub<DevicesHub>("/devices");

app.Run();
