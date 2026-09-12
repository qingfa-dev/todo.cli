public sealed class ListTasksHandlerTests
{
    [Fact]
    public void List_ShouldReturnTasks()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "Learn C#",
                Status = TaskStatus.Todo
            },
            new TaskItem
            {
                Id = 2,
                Description = "Build CLI",
                Status = TaskStatus.Done
            });

        var handler = new ListTasksHandler(store);

        var result = handler.Handle();

        Assert.True(result.Success);
        Assert.NotNull(result.Value);

        Assert.Equal(2, result.Value.Tasks.Count);
    }

    [Fact]
    public void List_ShouldFilterByStatus()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "One",
                Status = TaskStatus.Todo
            },
            new TaskItem
            {
                Id = 2,
                Description = "Two",
                Status = TaskStatus.Done
            },
            new TaskItem
            {
                Id = 3,
                Description = "Three",
                Status = TaskStatus.Todo
            });

        var handler = new ListTasksHandler(store);

        var result = handler.Handle(TaskStatus.Todo);

        Assert.True(result.Success);

        Assert.Equal(
            2,
            result.Value!.Tasks.Count);

        Assert.All(
            result.Value.Tasks,
            task => Assert.Equal(
                TaskStatus.Todo,
                task.Status));
    }

    [Fact]
    public void List_ShouldReturnCountsForAllStatuses()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "One",
                Status = TaskStatus.Todo
            },
            new TaskItem
            {
                Id = 2,
                Description = "Two",
                Status = TaskStatus.InProgress
            },
            new TaskItem
            {
                Id = 3,
                Description = "Three",
                Status = TaskStatus.Done
            },
            new TaskItem
            {
                Id = 4,
                Description = "Four",
                Status = TaskStatus.Todo
            });

        var handler = new ListTasksHandler(store);

        var result = handler.Handle();

        Assert.True(result.Success);

        Assert.Equal(2, result.Value!.TodoCount);
        Assert.Equal(1, result.Value.InProgressCount);
        Assert.Equal(1, result.Value.DoneCount);
    }

    [Fact]
    public void List_ShouldOrderById()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 3,
                Description = "Three",
                Status = TaskStatus.Todo
            },
            new TaskItem
            {
                Id = 1,
                Description = "One",
                Status = TaskStatus.Todo
            },
            new TaskItem
            {
                Id = 2,
                Description = "Two",
                Status = TaskStatus.Todo
            });

        var handler = new ListTasksHandler(store);

        var result = handler.Handle();

        Assert.Equal(
            [1, 2, 3],
            result.Value!.Tasks
                .Select(task => task.Id)
                .ToArray());
    }
}
