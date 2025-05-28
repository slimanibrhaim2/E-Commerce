using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;

namespace Communication.Application.Queries.GetAllMessages;

public record GetAllMessagesQuery() : IRequest<Result<List<MessageDTO>>>; 