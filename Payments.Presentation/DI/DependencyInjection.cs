using Payments.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using Payments.Application.Commands.CreatePayment;

namespace Payments.Presentation.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentsPresentation(this IServiceCollection services)
        {
            services.AddControllers()
                    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(PaymentController).Assembly));

            // Add MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly);
            });

            return services;
        }
    }
} 