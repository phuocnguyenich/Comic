using Comic.API.Code.AutoMapper;
using Comic.API.Code.Interfaces;
using Comic.API.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Comic.API.Code.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database configuration
builder.Services.AddDbContext<ComicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IComicService, ComicService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
	try
	{
        var dbContext = scope.ServiceProvider.GetRequiredService<ComicDbContext>();
        //dbContext.Database.Migrate();

        // Clear fake data
        //dbContext.ClearAllData();

        // Generate fake data
        dbContext.GenerateFakeData();
    }
	catch (Exception ex)
	{

		throw;
	}
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();