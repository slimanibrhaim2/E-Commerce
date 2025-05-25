using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.CreateBrand;

public record CreateBrandCommand(CreateBrandDTO Brand) : IRequest<Result<Guid>>; 