public sealed class AddTaskHandler
{
    private readonly ITaskStore _store;

    public AddTaskHandler(ITaskStore store)
    {
        _store = store;
    }

    public Result<TaskItem> Handle(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<TaskItem>.Fail(
                "Task description cannot be empty.");
        }

        var tasks = _store.Load();

        var nextId = tasks.Count == 0
            ? 1
            : tasks.Max(task => task.Id) + 1;

        var now = DateTime.UtcNow;

        var task = new TaskItem
        {
            Id = nextId,
            Description = description.Trim(),
            Status = TaskStatus.Todo,
            CreatedAt = now,
            UpdatedAt = now
        };

        tasks.Add(task);

        if (!_store.Save(tasks))
        {
            return Result<TaskItem>.Fail(
                "Could not save tasks.");
        }

        return Result<TaskItem>.Ok(task);
    }
}