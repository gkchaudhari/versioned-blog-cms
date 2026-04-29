


using CmsBackend.Domain.Dtos;

namespace CmsBackend.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        string Register(RegisterUser dto);
        string Login(Login dto);
    }
}
