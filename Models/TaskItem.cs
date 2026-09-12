public sealed class TaskItem
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = TaskStatus.Todo;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}