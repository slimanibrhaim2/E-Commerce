using Infrastructure;
using Users.Presentation;
using Users.Application.DI;
using Users.Infrastructure.DI;
using Users.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddUserInfrastructure();
builder.Services.AddControllers()
    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(UsersController).Assembly));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
