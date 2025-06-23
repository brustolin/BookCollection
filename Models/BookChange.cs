using BookTracker.Tools.ChangeTracker;

namespace BookTracker.Models;

public class BookChange : EntityChange
{
    public Book? Book { get; set; }
}