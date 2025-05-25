using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateBrand;

public record UpdateBrandCommand(Guid Id, CreateBrandDTO Brand) : IRequest<Result<bool>>; 