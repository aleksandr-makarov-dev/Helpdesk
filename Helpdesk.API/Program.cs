using Helpdesk.API.Domain;
using Helpdesk.API.Modules.Attachments;
using Helpdesk.API.Modules.Tickets;
using Microsoft.EntityFrameworkCore;
using Minio;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

// Tickets
builder.Services.AddScoped<TicketService>();

// Attachments
var minioAccessKey = "tz4azZwYuyQfxEUDwC01";
var minioSecretKey = "tpjxZqWpvTiNRI3FOZYZDYs414sN4B16sMtwNkoY";

builder.Services.AddMinio(configureClient=>configureClient
    .WithEndpoint("localhost:9000")
    .WithCredentials(minioAccessKey,minioSecretKey)
    .WithSSL(false)
    .Build());

builder.Services.AddScoped<IStorageService, MinioStorageService>();
builder.Services.AddScoped<AttachmentService>();

builder.Services.AddDbContext<ApplicationDbContext>((options) =>
{
    options.UseNpgsql("Host=localhost; Port=5432; Database=helpdesk_db; Username=postgres; Password=root123");
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.SeedDatabaseAsync().Wait();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
