using CmsBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CmsBackend.Domain.Dtos
{
    public record UserResponseDto(
       Guid Id,
       string Name,
       string Email,
       string Role
   );

    public record AssignRoleDto(
        Guid UserId,
        Roles Role
    );
}
