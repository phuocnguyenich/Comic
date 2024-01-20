using Comic.API.Areas.ActionFilters;
using Comic.API.Code.Helpers;
using Comic.API.Code.Identity;
using Comic.API.Code.Interfaces;
using Comic.API.Code.Services;
using Comic.API.Data;
using Comic.API.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database configuration
builder.Services.AddDbContext<ComicDbContext>(options =>
    options
    .UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging());
builder.Services.AddIdentity<AppUser, IdentityRole<int>>(o =>
{
    //o.Password.RequireDigit = true;
    //o.Password.RequireLowercase = false;
    //o.Password.RequireUppercase = false;
    //o.Password.RequireNonAlphanumeric = false;
    //o.Password.RequiredLength = 10;
    o.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ComicDbContext>()
.AddDefaultTokenProviders()
.AddTokenProvider("Auth", typeof(DataProtectorTokenProvider<AppUser>));

builder.Services.AddScoped<IComicService, ComicService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddSingleton<ICurrentUser, CurrentUser>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ValidationFilterAttribute>();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IViewCountQueue, ViewCounterQueueService>();
builder.Services.AddHostedService<ViewCountService>();
builder.Services.AddHostedService<ScheduledViewCountService>();
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200");
    });
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.ValidIssuer,
                ValidAudience = jwtSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });

// Configure token options for RefreshToken
builder.Services.Configure<DataProtectionTokenProviderOptions>("Auth", options =>
{
    options.TokenLifespan = TimeSpan.FromDays(jwtSettings.RefreshTokenExpirationHours); // Set your desired lifespan for refresh tokens
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
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();
app.Run();
