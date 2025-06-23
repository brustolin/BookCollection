using System;
using System.Text.Json.Serialization;
using BookTracker.Tools;
using BookTracker.Tools.ChangeTracker;

namespace BookTracker.Models;

[ChangeStorage<BookChange>()]
public class Book
{
    public int Id { get; set; }
    [BookPropertyMap(BookProperty.Title)]
    public string Title { get; set; } = String.Empty;

    [BookPropertyMap(BookProperty.ShortDescription)]
    public string ShortDescription { get; set; } = String.Empty;

    [BookPropertyMap(BookProperty.ReleaseDate)]
    public DateTime ReleaseDate { get; set; }
    public List<Author> Authors { get; set; } = new();
}

class BookPropertyMapAttribute : PropertyMapAttribute<BookProperty>
{
    public BookPropertyMapAttribute(BookProperty property) : base(property)
    {}
}

public enum BookProperty : int
{
    Title = 1,
    ShortDescription,
    ReleaseDate,
    Authors
}