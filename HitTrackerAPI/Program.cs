using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HitTrackerAPI.Data;
using HitTrackerAPI.Repositories;
using HitTrackerAPI.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// JWT
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer   = false,
            ValidateAudience = false,
            ClockSkew        = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

builder.Services.AddOpenApi();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ICalorieService, CalorieService>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();

var app = builder.Build();

// DB otomatik oluştur + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    var quoteRepo = scope.ServiceProvider.GetRequiredService<IQuoteRepository>();
    await quoteRepo.SeedAsync();

    var exerciseSeed = new HitTrackerAPI.Repositories.ExerciseSeedService(db);
    await exerciseSeed.SeedAsync(
        Path.Combine(builder.Environment.ContentRootPath, "exercises_data")
    );
}

// Scalar
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "HIT Tracker API";
});

app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "exercises_data")
    ),
    RequestPath = "/exercise-images"
});

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();

app.Run();