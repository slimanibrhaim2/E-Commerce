using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.CreateService;
 
public record CreateServiceCommand(ServiceDTO ServiceDto) : IRequest<Result<Guid>>; 