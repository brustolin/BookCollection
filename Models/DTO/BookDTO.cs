using BookTracker.Models;

namespace BookStore.Models.DTO;

public class BookDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public DateTime ReleaseDate { get; set; }

    public List<AuthorDTO> Authors { get; set; } = new();

    public BookDTO()
    {
        
    }

    public BookDTO(Book book)
    {
        Id = book.Id;
        Title = book.Title;
        ShortDescription = book.ShortDescription;
        ReleaseDate = book.ReleaseDate;
        Authors = book.Authors.Select(a => new AuthorDTO(a)).ToList();
    }
}