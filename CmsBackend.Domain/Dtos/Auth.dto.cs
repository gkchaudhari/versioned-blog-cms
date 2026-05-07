namespace CmsBackend.Domain.Dtos
{
    public class RegisterUser
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class Login
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
