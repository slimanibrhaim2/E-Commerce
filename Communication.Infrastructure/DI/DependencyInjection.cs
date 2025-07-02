using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Communication.Domain.Repositories;
using Communication.Domain.Interfaces;
using Communication.Infrastructure.Repositories;
using Communication.Infrastructure.MessageBus;
using Communication.Infrastructure.Configuration;
using Communication.Infrastructure.HealthChecks;
using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;
using Communication.Infrastructure.Mapping.Mappers;

namespace Communication.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommunicationInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register Repositories
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IConversationMemberRepository, ConversationMemberRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IAttachmentTypeRepository, AttachmentTypeRepository>();
            services.AddScoped<IBaseContentRepository, BaseContentRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            // Register Mappers
            services.AddScoped<IMapper<ConversationDAO, Conversation>, ConversationMapper>();
            services.AddScoped<IMapper<MessageDAO, Message>, MessageMapper>();
            services.AddScoped<IMapper<CommentDAO, Comment>, CommentMapper>();
            services.AddScoped<IMapper<ConversationMemberDAO, ConversationMember>, ConversationMemberMapper>();
            services.AddScoped<IMapper<AttachmentDAO, Attachment>, AttachmentMapper>();
            services.AddScoped<IMapper<AttachmentTypeDAO, AttachmentType>, AttachmentTypeMapper>();
            services.AddScoped<IMapper<BaseContentDAO, BaseContent>, BaseContentMapper>();
            services.AddScoped<IMapper<ReviewDAO, Review>, ReviewMapper>();

            // Configure and register message bus
            services.Configure<MessageBusSettings>(
                configuration.GetSection("MessageBus"));
            services.AddSingleton<IReviewEvaluationPublisher, RabbitMQReviewEvaluationPublisher>();

            // Register health checks
            services.AddHealthChecks()
                .AddCheck<RabbitMQHealthCheck>("rabbitmq_health_check");

            return services;
        }
    }
} 