public sealed class ChangeTaskStatusHandler
{
    private readonly ITaskStore _store;

    public ChangeTaskStatusHandler(ITaskStore store)
    {
        _store = store;
    }

    public Result<TaskItem> Handle(
        int id,
        string status)
    {
        if (!TaskStatus.IsValid(status))
        {
            return Result<TaskItem>.Fail(
                $"Unknown status: {status}");
        }

        var tasks = _store.Load();

        var task = tasks.FirstOrDefault(
            task => task.Id == id);

        if (task is null)
        {
            return Result<TaskItem>.Fail(
                $"Task #{id} was not found.");
        }

        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;

        if (!_store.Save(tasks))
        {
            return Result<TaskItem>.Fail(
                "Could not save tasks.");
        }

        return Result<TaskItem>.Ok(task);
    }
}