using CmsBackend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CmsBackend.Domain.Dtos
{
    public record BlogCreateDto
    {
        [Required]
        public required string Title { get; init; }
        public required string Content { get; init; }
    }

    public record BlogUpdateDto
    {
        public string? Title { get; init; }
        public string? Content { get; init; }

        public BlogStatus Status { get; init; }
    }

    public record BlogResponseDto
    {
        public Guid Id { get; init; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? AuthorName { get; set; }
        public string? Status { get; set; }

    }
}
