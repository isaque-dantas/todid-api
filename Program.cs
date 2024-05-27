using System.Text;
using TodoAPI;
using TodoAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TodoAPI.Models;
using TodoAPI.Requests;
using TodoAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlite<TodoContext>("Data Source=TodoDb.db");

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
            IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes(AppSettingsService.JwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

UserEndpoints.Map(app);
TodoItemEndpoints.Map(app);
TodoListEndpoints.Map(app);

app.Run();