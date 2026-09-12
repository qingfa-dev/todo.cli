public sealed class DeleteTaskHandler
{
    private readonly ITaskStore _store;

    public DeleteTaskHandler(ITaskStore store)
    {
        _store = store;
    }

    public Result<int> Handle(int id)
    {
        var tasks = _store.Load();

        var task = tasks.FirstOrDefault(
            task => task.Id == id);

        if (task is null)
        {
            return Result<int>.Fail(
                $"Task #{id} was not found.");
        }

        tasks.Remove(task);

        if (!_store.Save(tasks))
        {
            return Result<int>.Fail(
                "Could not save tasks.");
        }

        return Result<int>.Ok(id);
    }
}