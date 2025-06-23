using BookTracker.Models;
namespace BookStore.Models.DTO;

public class AuthorDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public AuthorDTO() { }

    public AuthorDTO(Author author)
    {
        Id = author.Id;
        Name = author.Name;
    }
}