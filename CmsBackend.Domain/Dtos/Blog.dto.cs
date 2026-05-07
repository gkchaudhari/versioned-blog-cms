using System.ComponentModel.DataAnnotations;

namespace CmsBackend.Domain.Dtos
{
    public record BlogCreateDto
    {
        [Required]
        public string Title { get; init; }
        public string Content { get; init; }
    }

    public record BlogResponseDto
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Content { get; init; }
        public string AuthorName { get; init; }
        public string Status { get; init; }

    }
}
