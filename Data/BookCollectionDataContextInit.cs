using BookTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Data;

public partial class BookCollectionDataContext : DbContext
{
    /// <summary>
    /// Initializes the database with sample data because project uses in-memory database.
    /// </summary>
    public static void Initialize(BookCollectionDataContext context)
    {
        context.Database.EnsureCreated();

        if (context.Books.Any())
            return;

        var authors = new List<Author>
        {
            new Author { Name = "George Orwell" },
            new Author { Name = "J.R.R. Tolkien" },
            new Author { Name = "F. Scott Fitzgerald" },
            new Author { Name = "Harper Lee" },
            new Author { Name = "Herman Melville" },
            new Author { Name = "Jane Austen" },
            new Author { Name = "Mary Shelley" },
            new Author { Name = "Leo Tolstoy" },
            new Author { Name = "Gabriel García Márquez" },
            new Author { Name = "J.D. Salinger" },
            new Author { Name = "Aldous Huxley" },
            new Author { Name = "Ray Bradbury" },
            new Author { Name = "John Steinbeck" },
            new Author { Name = "Paulo Coelho" },
            new Author { Name = "Victor Hugo" },
            new Author { Name = "Fyodor Dostoevsky" },
            new Author { Name = "Miguel de Cervantes" },
            new Author { Name = "Emily Brontë" },
            new Author { Name = "Homer" },
            new Author { Name = "Dante Alighieri" },
        };

        context.Authors.AddRange(authors);

        var books = new List<Book>
        {
            new Book
            {
                Title = "1984",
                ShortDescription = "A dystopian social science fiction novel and cautionary tale.",
                ReleaseDate = new DateTime(1949, 6, 8),
                Authors = new List<Author> { authors[0] }
            },
            new Book
            {
                Title = "The Lord of the Rings",
                ShortDescription = "An epic high-fantasy novel set in Middle-earth.",
                ReleaseDate = new DateTime(1954, 7, 29),
                Authors = new List<Author> { authors[1] }
            },
            new Book
            {
                Title = "The Great Gatsby",
                ShortDescription = "A novel about the American dream and societal excess in the 1920s.",
                ReleaseDate = new DateTime(1925, 4, 10),
                Authors = new List<Author> { authors[2] }
            },
            new Book
            {
                Title = "To Kill a Mockingbird",
                ShortDescription = "A novel on racial injustice in the Deep South.",
                ReleaseDate = new DateTime(1960, 7, 11),
                Authors = new List<Author> { authors[3] }
            },
            new Book
            {
                Title = "Moby-Dick",
                ShortDescription = "A sailor's narrative of the obsessive quest for revenge on a white whale.",
                ReleaseDate = new DateTime(1851, 10, 18),
                Authors = new List<Author> { authors[4] }
            },
            new Book
            {
                Title = "Pride and Prejudice",
                ShortDescription = "A romantic novel about manners and matrimonial machinations.",
                ReleaseDate = new DateTime(1813, 1, 28),
                Authors = new List<Author> { authors[5] }
            },
            new Book
            {
                Title = "Frankenstein",
                ShortDescription = "A gothic novel about the dangers of playing God.",
                ReleaseDate = new DateTime(1818, 1, 1),
                Authors = new List<Author> { authors[6] }
            },
            new Book
            {
                Title = "War and Peace",
                ShortDescription = "A historical novel that intertwines the lives of families during the Napoleonic wars.",
                ReleaseDate = new DateTime(1869, 1, 1),
                Authors = new List<Author> { authors[7] }
            },
            new Book
            {
                Title = "One Hundred Years of Solitude",
                ShortDescription = "A multi-generational story of the Buendía family in the fictional town of Macondo.",
                ReleaseDate = new DateTime(1967, 5, 30),
                Authors = new List<Author> { authors[8] }
            },
            new Book
            {
                Title = "The Catcher in the Rye",
                ShortDescription = "A story about teenage alienation and angst in post-war America.",
                ReleaseDate = new DateTime(1951, 7, 16),
                Authors = new List<Author> { authors[9] }
            },
            new Book
            {
                Title = "Brave New World",
                ShortDescription = "A futuristic society controlled by technology and conditioning.",
                ReleaseDate = new DateTime(1932, 1, 1),
                Authors = new List<Author> { authors[10] }
            },
            new Book
            {
                Title = "Fahrenheit 451",
                ShortDescription = "A dystopian novel where books are outlawed and burned.",
                ReleaseDate = new DateTime(1953, 10, 19),
                Authors = new List<Author> { authors[11] }
            },
            new Book
            {
                Title = "The Grapes of Wrath",
                ShortDescription = "A depiction of the Dust Bowl migration during the Great Depression.",
                ReleaseDate = new DateTime(1939, 4, 14),
                Authors = new List<Author> { authors[12] }
            },
            new Book
            {
                Title = "The Alchemist",
                ShortDescription = "A journey of a young shepherd in search of his personal legend.",
                ReleaseDate = new DateTime(1988, 4, 15),
                Authors = new List<Author> { authors[13] }
            },
            new Book
            {
                Title = "Les Misérables",
                ShortDescription = "A sweeping story of justice, redemption, and love in 19th-century France.",
                ReleaseDate = new DateTime(1862, 4, 3),
                Authors = new List<Author> { authors[14] }
            },
            new Book
            {
                Title = "Crime and Punishment",
                ShortDescription = "A psychological drama about guilt and redemption.",
                ReleaseDate = new DateTime(1866, 1, 1),
                Authors = new List<Author> { authors[15] }
            },
            new Book
            {
                Title = "Don Quixote",
                ShortDescription = "A comedic tale of a man who becomes a self-declared knight.",
                ReleaseDate = new DateTime(1605, 1, 16),
                Authors = new List<Author> { authors[16] }
            },
            new Book
            {
                Title = "Wuthering Heights",
                ShortDescription = "A dark romance set on the Yorkshire moors.",
                ReleaseDate = new DateTime(1847, 12, 1),
                Authors = new List<Author> { authors[17] }
            },
            new Book
            {
                Title = "The Iliad",
                ShortDescription = "An ancient Greek epic poem set during the Trojan War.",
                ReleaseDate = new DateTime(2000, 1, 1),
                Authors = new List<Author> { authors[18] }
            },
            new Book
            {
                Title = "The Divine Comedy",
                ShortDescription = "An epic poem describing Dante's journey through Hell, Purgatory, and Paradise.",
                ReleaseDate = new DateTime(1320, 1, 1),
                Authors = new List<Author> { authors[19] }
            }
        };

        context.Books.AddRange(books);
        context.SaveChanges();


        // Change a few book properties to trigger change tracking
        books[0].ShortDescription = "A dystopian novel set in a totalitarian society that uses surveillance and propaganda to control its citizens.";
        books[1].ReleaseDate = new DateTime(1950, 7, 29);
        books[2].Title = "The Great Gatsby: A Novel of the Jazz Age";
        books[3].ShortDescription = "A novel that explores themes of racial injustice, moral growth, and compassion in the Deep South during the 1930s.";
        books[4].ReleaseDate = new DateTime(1850, 10, 18);
        books[5].Title = "Pride and Prejudice: A Classic Romance";
        books[6].ShortDescription = "A gothic novel that tells the story of Victor Frankenstein, a young scientist who creates a grotesque creature in an unorthodox experiment.";
        books[7].ReleaseDate = new DateTime(1860, 1, 1);
        books[8].Title = "One Hundred Years of Solitude: A Magical Realism Masterpiece";
        books[9].Authors.Clear();

        context.SaveChanges();
    }
}