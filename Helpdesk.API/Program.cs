using System.Text;
using Helpdesk.API.Configuration;
using Helpdesk.API.Domain;
using Helpdesk.API.Modules.Attachments;
using Helpdesk.API.Modules.Tickets;
using Helpdesk.API.Modules.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("ApplicationOptions"));


var applicationOptions = builder.Services.BuildServiceProvider()
    .GetRequiredService<IOptions<ApplicationOptions>>().Value;

if (applicationOptions is null)
{
    throw new Exception("Application Options is null");
}


// Tickets
builder.Services.AddScoped<TicketService>();

builder.Services.AddMinio(configureClient=>configureClient
    .WithEndpoint(applicationOptions.MinioEndpoint)
    .WithCredentials(applicationOptions.MinioAccessKey,applicationOptions.MinioSecretKey)
    .WithSSL(false)
    .Build());

builder.Services.AddScoped<IStorageService, MinioStorageService>();
builder.Services.AddScoped<AttachmentService>();

// Users
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JsonWebTokenService>();

builder.Services.AddDbContext<ApplicationDbContext>((options) =>
{
    // TODO: move to secret store
    options.UseNpgsql(applicationOptions.ConnectionString);
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>((options) =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddJwtBearer((options) =>
    {
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateIssuer = false;
        options.TokenValidationParameters.IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(applicationOptions.JwtSecretKey));
        options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Helpdesk API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.SeedDatabaseAsync().Wait();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
