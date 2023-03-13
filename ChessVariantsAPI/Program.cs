using ChessVariantsAPI;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddGameOrganzation();
builder.Services.AddSingleton<DatabaseService>(new TestDatabaseService(builder.Configuration["MongoDatabase:ConnectionString"]));

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(options =>
            options.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .WithMethods("GET", "POST")
            .AllowCredentials());
}



app.UseHttpsRedirection();

//app.UseAuthorization();
//app.UseAuthentication();

app.MapControllers();
app.MapHubs();

app.Run();
