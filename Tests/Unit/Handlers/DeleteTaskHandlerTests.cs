public sealed class DeleteTaskHandlerTests
{
    [Fact]
    public void Delete_ShouldRemoveTask()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "Delete me",
                Status = TaskStatus.Todo
            });

        var handler = new DeleteTaskHandler(store);

        var result = handler.Handle(1);

        Assert.True(result.Success);
        Assert.Equal(1, result.Value);

        var tasks = store.Load();

        Assert.Empty(tasks);
    }

    [Fact]
    public void Delete_ShouldFail_WhenTaskDoesNotExist()
    {
        var store = new InMemoryTaskStore();
        var handler = new DeleteTaskHandler(store);

        var result = handler.Handle(999);

        Assert.False(result.Success);

        Assert.Equal(
            "Task #999 was not found.",
            result.Error);
    }

    [Fact]
    public void Delete_ShouldFail_WhenStoreCannotSave()
    {
        var store = new InMemoryTaskStore
        {
            SaveSucceeds = false
        };

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "Delete me",
                Status = TaskStatus.Todo
            });

        var handler = new DeleteTaskHandler(store);

        var result = handler.Handle(1);

        Assert.False(result.Success);

        Assert.Equal(
            "Could not save tasks.",
            result.Error);
    }
}
