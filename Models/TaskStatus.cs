public static class TaskStatus
{
    public const string Todo = "todo";

    public const string InProgress = "in-progress";

    public const string Done = "done";

    public static string? Normalize(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "todo" => Todo,
            "in-progress" => InProgress,
            "inprogress" => InProgress,
            "done" => Done,
            _ => null
        };
    }

    public static bool IsValid(string value)
    {
        return value is Todo or InProgress or Done;
    }
}