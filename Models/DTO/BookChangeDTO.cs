using BookTracker.Models;
using BookTracker.Tools.ChangeTracker;

namespace BookStore.Models.DTO;

public class BookChangeDTO
{
     public int Id { get; set; }
    public int SourceId { get; set; }
    public string? BookName { get; set; }
    public string? Property { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangeType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    

    public BookChangeDTO()
    {
    }

    public BookChangeDTO(BookChange bookChange)
    {
        Id = bookChange.Id;
        SourceId = bookChange.SourceId;
        BookName = bookChange.Book?.Title ?? string.Empty;
        Property = ((BookProperty)bookChange.PropertyId).ToString();
        OldValue = bookChange.OldValue;
        NewValue = bookChange.NewValue;
        ChangeType = bookChange.ChangeType.ToString();
        Timestamp = bookChange.Timestamp;
    }
}