namespace BookTracker.Tools.ModelBinders;

public interface IStringFactory<T>
where T : notnull
{
    static abstract T Create(string value);
}
