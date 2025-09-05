using BondTalesChat_Server.Data;
using BondTalesChat_Server.DatabaseInitializer;
using BondTalesChat_Server.Hubs;
using BondTalesChat_Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
// Need to look this package's usage. 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "https://your-frontend-domain.onrender.com", // Replace with your actual frontend domain
                "https://your-frontend-domain.vercel.app",   // Replace with your actual frontend domain
                "https://your-frontend-domain.netlify.app"   // Replace with your actual frontend domain
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

});

builder.Services.AddScoped<JwtService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents 
        {
            OnMessageReceived = context =>
            {
                // context.Token = context.Request.Cookies["token"];
                var token = context.Request.Cookies["token"];

                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
        
    });

// Add Services to the container.

builder.Services.AddSingleton<OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddSignalR(o => { o.EnableDetailedErrors = true; });
builder.Services.AddSingleton<ICallStateService, CallStateService>();
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Configure the SQL scripts folder path
var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
var sqlFolderPath = Path.Combine(projectRoot, "BondTalesChat-Database", "create-tables-sql");

Console.WriteLine(sqlFolderPath);

//run a method to execute the SQL files and create table during initialization
DatabaseInitializer.Initializer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlFolderPath);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<CallHub>("/hubs/call");
app.Run();
