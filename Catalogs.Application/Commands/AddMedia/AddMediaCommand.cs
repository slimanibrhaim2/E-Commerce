using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.AddMedia
{
    public record AddMediaCommand(Guid ItemId, CreateMediaDTO Mediadto) : IRequest<Result<Guid>>;
} 