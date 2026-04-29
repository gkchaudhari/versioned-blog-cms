using CmsBackend.Application.Users.Interfaces;
using CmsBackend.Domain.Dtos;
using CmsBackend.Infrastructure.Data;

namespace CmsBackend.Application.Users.Services
{
    public class UserService:IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public List<UserResponseDto> GetAllUsers()
        {

            var users = _context.Users
                .Select(u => new UserResponseDto(
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role.ToString()
                ))
                .ToList();

            return users;
        }

        public void AssignRole(AssignRoleDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == dto.UserId);

            if (user == null)
                throw new Exception("User not found");

            user.Role = dto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }
    }
}
