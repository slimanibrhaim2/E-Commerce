using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Payments.Infrastructure.Mapping.Mappers;
using Payments.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentInfrastructure(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentStatusRepository, PaymentStatusRepository>();

            // Register Mappers
            services.AddScoped<IMapper<PaymentMethodDAO, PaymentMethod>, PaymentMethodMapper>();
            services.AddScoped<IMapper<PaymentDAO, Payment>, PaymentMapper>();
            services.AddScoped<IMapper<PaymentStatusDAO, PaymentStatus>, PaymentStatusMapper>();

            return services;
        }
    }
}
