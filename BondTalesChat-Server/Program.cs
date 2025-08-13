using BondTalesChat_Server.Data;
using BondTalesChat_Server.DatabaseInitializer;
using BondTalesChat_Server.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.Run();
