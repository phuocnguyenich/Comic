using Comic.API.Code.Interfaces;
using Comic.API.Code.Services;
using Comic.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database configuration
builder.Services.AddDbContext<ComicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IComicService, ComicService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IViewCountQueue, ViewCounterQueueService>();
builder.Services.AddHostedService<ViewCountService>();
builder.Services.AddHostedService<ScheduledViewCountService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200");
    });
});

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
        // dbContext.GenerateFakeData();
    }
    catch (Exception ex)
    {

        throw;
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();
app.Run();
