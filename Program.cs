using Azure.Storage.Blobs;
using BlobStorage.Models;
using BlobStorage.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationContext>(op=>op.UseSqlServer(builder.Configuration.GetConnectionString("connectToDb")));

builder.Services.AddScoped<DataBaseService>();

builder.Services.AddSingleton(new BlobServiceClient(builder.Configuration.GetConnectionString("connectToBlob")));

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

var app = builder.Build();
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

app.Run();
