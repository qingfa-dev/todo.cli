public sealed class ListTasksHandler
{
    private readonly ITaskStore _store;

    public ListTasksHandler(ITaskStore store)
    {
        _store = store;
    }

    public Result<ListTasksResult> Handle(
        string? status = null)
    {
        var tasks = _store.Load();

        var filteredTasks = tasks
            .Where(task =>
                status is null ||
                task.Status == status)
            .OrderBy(task => task.Id)
            .ToList();

        var result = new ListTasksResult(
            filteredTasks,
            tasks.Count(task => task.Status == TaskStatus.Todo),
            tasks.Count(task => task.Status == TaskStatus.InProgress),
            tasks.Count(task => task.Status == TaskStatus.Done));

        return Result<ListTasksResult>.Ok(result);
    }
}