using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoAPI;
using TodoAPI.Authentication;
using TodoAPI.Endpoints;
using TodoAPI.Models;
using TodoAPI.Requests;
using TodoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(System.Net.IPAddress.Parse("10.0.0.112"), 8000);
});

const string corsPolicy = "corsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy  =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddDbContext<TodoContext>();

builder.AddFluentValidationEndpointFilter();
builder.Services.AddScoped<IValidator<UpdateTodoListRequest>, UpdateTodoListRequest.Validator>();
builder.Services.AddScoped<IValidator<TodoListDto>, TodoListDto.Validator>();
builder.Services.AddScoped<IValidator<TodoItemDto>, TodoItemDto.Validator>();
builder.Services.AddScoped<IValidator<UserDto>, UserDto.Validator>();

builder.Services.AddScoped<TodoItemService>();
builder.Services.AddScoped<TodoListService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettingsService.JwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();

UserEndpoints.Map(app);
TodoItemEndpoints.Map(app);
TodoListEndpoints.Map(app);

app.Run();
