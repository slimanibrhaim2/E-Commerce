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
using Payments.Presentation.DI;
using Payments.Application.DI;
using Payments.Infrastructure.DI;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Authentication.Controllers;
using WebApi.Authentication.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add Authentication Services
builder.Services.AddAuthenticationServices(builder.Configuration);


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
builder.Services.AddPaymentsPresentation();
builder.Services.AddPaymentsApplicationServices();
builder.Services.AddPaymentInfrastructure();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-Commerce API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
