using MediatR;
using Core.Result;

namespace Shared.Contracts.Queries;

public record GetItemPriceByIdQuery(Guid ItemId) : IRequest<Result<GetItemPriceByIdResponse>>;

public record GetItemPriceByIdResponse(double Price); 