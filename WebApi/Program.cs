using Infrastructure;
using Users.Presentation;
using Users.Application.DI;
using Users.Infrastructure.DI;
using Users.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Users.Presentation.DI;
using Catalogs.Presentation.DI;
using Catalogs.Infrastructure.DI;
using Communication.Application.DI;
using Communication.Infrastructure.DI;
using Communication.Presentation.DI;
using Shoppings.Application.DI;
using Shoppings.Infrastructure.DI;
using Shoppings.Presentation.DI;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddUsersApplication();
builder.Services.AddUserInfrastructure();
builder.Services.AddUserPresentation();
builder.Services.AddCatalogsInfrastructure(); 
builder.Services.AddCatalogPresentation(); 
builder.Services.AddCommunicationApplicationServices();
builder.Services.AddCommunicationInfrastructure();
builder.Services.AddShoppingsPresentation();
builder.Services.AddShoppingsApplication();
builder.Services.AddShoppingsInfrastructure();
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
