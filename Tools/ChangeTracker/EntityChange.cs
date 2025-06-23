namespace BookTracker.Tools.ChangeTracker;

public abstract class EntityChange
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public int PropertyId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public EntityChangeType ChangeType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}