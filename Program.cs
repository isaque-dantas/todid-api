using TodoAPI;
using TodoAPI.Services;
using FluentValidation;
using TodoAPI.Models;
using TodoAPI.Requests;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlite<TodoContext>("Data Source=TodoDb.db");

builder.AddFluentValidationEndpointFilter();
builder.Services.AddScoped<IValidator<UpdateTodoListRequest>, UpdateTodoListRequest.Validator>();
builder.Services.AddScoped<IValidator<TodoListDto>, TodoListDto.Validator>();
builder.Services.AddScoped<IValidator<TodoItemDto>, TodoItemDto.Validator>();

builder.Services.AddScoped<TodoItemService>();
builder.Services.AddScoped<TodoListService>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin_greetings", policy => policy.RequireRole("admin"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

TodoItemEndpoints.Map(app);
TodoListEndpoints.Map(app);

app.Run();