using Infrastructure.Common;
using Infrastructure.Models;
using Users.Domain.Entities;

namespace Users.Infrastructure.Mapping;

public class UserMapper : BaseMapper<UserDAO, User>
{
    public override User Map(UserDAO source)
    {
        return SafeMap(source, s => new User
        {
            Id = s.Id,
            FirstName = s.FirstName,
            MiddleName = null,
            LastName = s.LastName,
            PhoneNumber = s.PhoneNumber,
            Email = s.Email,
            Rate = null,
            ProfilePhoto = s.ProfilePhoto,
            Description = null,
        });
    }

    public override UserDAO MapBack(User target)
    {
        return SafeMapBack(target, t => new UserDAO
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            PhoneNumber = t.PhoneNumber,
            Email = t.Email,
            ProfilePhoto = t.ProfilePhoto,
        });
    }
} 