using Core.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.DTOs;

namespace Users.Application.Command.UpdateUser
{
    public record UpdateUserCommand(
        Guid Id,
        CreateUserDTO User
        ) : IRequest<Result>;

}
