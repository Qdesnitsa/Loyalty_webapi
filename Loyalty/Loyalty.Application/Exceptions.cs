namespace Loyalty.Application;

public class InvalidCommandException : Exception
{
    public string Title { get; }
    public string Type { get; }

    public InvalidCommandException(string title, string type) : base(title)
    {
        Title = title;
        Type = type;
    }
}

public class InvalidQueryException : Exception
{
    public string Title { get; }
    public string Type { get; }

    public InvalidQueryException(string title, string type) : base(title)
    {
        Title = title;
        Type = type;
    }
}

public static class ExceptionTypes
{
    public const string ResourceNotFound = "ResourceNotFound";
    public const string ResourceAlreadyExists = "ResourceAlreadyExists";
}
