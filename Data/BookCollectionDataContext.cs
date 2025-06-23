using Microsoft.EntityFrameworkCore;
using BookTracker.Models;
using BookTracker.Tools.ChangeTracker;

namespace BookTracker.Data;

public partial class BookCollectionDataContext : DbContext
{
    private readonly EntityChangeTracker _changeTracker;

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<BookChange> BookChanges { get; set; }

    public BookCollectionDataContext(DbContextOptions<BookCollectionDataContext> options, EntityChangeTracker changeTracker)
        : base(options)
    {
        _changeTracker = changeTracker;
        _changeTracker.RegisterEntity<Book, BookChange>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Authors)
            .WithMany(a => a.Books)
            .UsingEntity<BookAuthor>(
                j => j.HasOne(ba => ba.Author)
                    .WithMany()
                    .HasForeignKey(ba => ba.AuthorId),
                j => j.HasOne(ba => ba.Book)
                    .WithMany()
                    .HasForeignKey(ba => ba.BookId),
                j =>
                {
                    j.HasKey(ba => new { ba.BookId, ba.AuthorId });
                });

        modelBuilder.Entity<BookChange>()
            .HasOne(bc => bc.Book)
            .WithMany()
            .HasForeignKey(bc => bc.SourceId);
    }

    public override int SaveChanges()
    {
        TrackChanges();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TrackChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void TrackChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted).ToList();
        
        foreach (var entry in entries)
        {
            if (entry.Entity is Book book &&
                entry.OriginalValues.ToObject() is Book originalBook &&
                entry.CurrentValues.ToObject() is Book currentBook)
            {
                var changes = _changeTracker.DetectChanges<Book, BookChange>(originalBook, currentBook);
                this.BookChanges.AddRange(changes);
            }
            else if (entry.Entity is BookAuthor bookAuthor)
            {
                // If the book was just added, we dont track changes for authors.
                if (entries.Any(e => e.Entity is Book book && book.Id == bookAuthor.BookId && e.State == EntityState.Added))
                    continue;

                var change = new BookChange
                {
                    SourceId = bookAuthor.BookId,
                    ChangeType = entry.State == EntityState.Added ? EntityChangeType.Created : EntityChangeType.Deleted,
                    PropertyId = (int)BookProperty.Authors,
                };

                if (entry.State == EntityState.Added) { change.NewValue = bookAuthor.Author.Name; }
                else if (entry.State == EntityState.Deleted) { change.OldValue = bookAuthor.Author.Name; }

                this.BookChanges.Add(change);
            }
        }
    }
}