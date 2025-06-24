using Communication.Domain.Repositories;
using Communication.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Common;
using Communication.Infrastructure.Mapping.Mappers;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommunicationInfrastructure(this IServiceCollection services)
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

            return services;
        }
    }
} 