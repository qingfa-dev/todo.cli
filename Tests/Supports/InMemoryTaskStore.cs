public sealed class InMemoryTaskStore : ITaskStore
{
    private List<TaskItem> _tasks = [];

    public bool SaveSucceeds { get; set; } = true;

    public List<TaskItem> Load()
    {
        return _tasks
            .Select(task => new TaskItem
            {
                Id = task.Id,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            })
            .ToList();
    }

    public bool Save(List<TaskItem> tasks)
    {
        if (!SaveSucceeds)
        {
            return false;
        }

        _tasks = tasks
            .Select(task => new TaskItem
            {
                Id = task.Id,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            })
            .ToList();

        return true;
    }

    public void Seed(params TaskItem[] tasks)
    {
        _tasks = tasks
            .Select(task => new TaskItem
            {
                Id = task.Id,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            })
            .ToList();
    }
}
