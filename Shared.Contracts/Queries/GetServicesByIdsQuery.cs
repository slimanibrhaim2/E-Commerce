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
    public record GetServicesByIdsQuery(IEnumerable<Guid> ServiceIds, PaginationParameters Parameters)
    : IRequest<Result<PaginatedResult<ServiceDetailsDTO>>>;
}
