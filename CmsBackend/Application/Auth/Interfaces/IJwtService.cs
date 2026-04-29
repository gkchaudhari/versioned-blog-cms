using CmsBackend.Domain.Entiites;

namespace CmsBackend.Application.Auth.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}