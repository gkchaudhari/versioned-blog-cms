using CmsBackend.Application.Auth.Interfaces;
using CmsBackend.Domain.Dtos;
using CmsBackend.Domain.Entiites;
using CmsBackend.Domain.Enums;
using CmsBackend.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace CmsBackend.Application.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwt;

        public AuthService(AppDbContext context, IJwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public string Register(RegisterUser dto)
        {
            if (_context.Users.Any(x => x.Email == dto.Email))
                throw new UnauthorizedAccessException("User already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Role = Roles.User,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return _jwt.GenerateToken(user);
        }

        public string Login(Login dto)
        {
            if (dto.Email == null || dto.Password == null)
            {
                throw new Exception("Invalid Credential");
            }

            var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

            if (user == null)
            {
                throw new Exception("User doest not exists");
            }

            if (user == null || user.PasswordHash != HashPassword(dto.Password))
                throw new Exception("Invalid credentials");

            return _jwt.GenerateToken(user);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }
    }
}
