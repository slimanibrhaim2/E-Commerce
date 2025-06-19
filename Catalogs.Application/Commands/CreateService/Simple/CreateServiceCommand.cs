using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.CreateService;
 
public record CreateServiceCommand(CreateServiceDTO Service, Guid UserId) : IRequest<Result<Guid>>; 