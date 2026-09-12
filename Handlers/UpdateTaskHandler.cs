public sealed class UpdateTaskHandler
{
    private readonly ITaskStore _store;

    public UpdateTaskHandler(ITaskStore store)
    {
        _store = store;
    }

    public Result<TaskItem> Handle(
        int id,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<TaskItem>.Fail(
                "Task description cannot be empty.");
        }

        var tasks = _store.Load();

        var task = tasks.FirstOrDefault(
            task => task.Id == id);

        if (task is null)
        {
            return Result<TaskItem>.Fail(
                $"Task #{id} was not found.");
        }

        task.Description = description.Trim();
        task.UpdatedAt = DateTime.UtcNow;

        if (!_store.Save(tasks))
        {
            return Result<TaskItem>.Fail(
                "Could not save tasks.");
        }

        return Result<TaskItem>.Ok(task);
    }
}