using MediatR;
using Shared.Contracts.DTOs;
using Core.Result;
using Core.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Queries
{
    public record GetProductsByIdsQuery(IEnumerable<Guid> ProductIds, PaginationParameters Parameters)
      : IRequest<Result<PaginatedResult<ProductDetailsDTO>>>;
}
