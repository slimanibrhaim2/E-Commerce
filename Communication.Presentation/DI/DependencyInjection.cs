using Communication.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Reflection;
using Communication.Application.Commands.CreateConversation;

namespace Communication.Presentation.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommunicationPresentation(this IServiceCollection services)
        {
            services.AddControllers()
                    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(AttachmentController).Assembly));

            // Add MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(CreateConversationCommand).Assembly);
            });

            return services;
        }
    }
} 