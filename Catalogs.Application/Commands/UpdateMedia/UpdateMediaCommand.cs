using Catalogs.Application.DTOs;
using MediatR;
using Core.Result;
using System;

namespace Catalogs.Application.Commands.UpdateMedia
{
    public record UpdateMediaCommand(Guid Id, CreateMediaDTO Media) : IRequest<Result<bool>>;
} 