using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteBrand;

public record DeleteBrandCommand(Guid Id) : IRequest<Result<bool>>; 