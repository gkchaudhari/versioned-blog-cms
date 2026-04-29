using CmsBackend.Domain.Dtos;

namespace CmsBackend.Application.Users.Interfaces
{
    public interface IUserService
    {
        List<UserResponseDto> GetAllUsers();
        void AssignRole(AssignRoleDto dto);
    }
}
