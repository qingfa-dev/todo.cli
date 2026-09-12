public sealed class UpdateTaskHandlerTests
{
    [Fact]
    public void Update_ShouldChangeDescription()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "Old description",
                Status = TaskStatus.Todo
            });

        var handler = new UpdateTaskHandler(store);

        var result = handler.Handle(
            1,
            "New description");

        Assert.True(result.Success);
        Assert.Equal(
            "New description",
            result.Value!.Description);
    }

    [Fact]
    public void Update_ShouldTrimDescription()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "Old",
                Status = TaskStatus.Todo
            });

        var handler = new UpdateTaskHandler(store);

        var result = handler.Handle(
            1,
            "  New description  ");

        Assert.True(result.Success);

        Assert.Equal(
            "New description",
            result.Value!.Description);
    }

    [Fact]
    public void Update_ShouldFail_WhenTaskDoesNotExist()
    {
        var store = new InMemoryTaskStore();
        var handler = new UpdateTaskHandler(store);

        var result = handler.Handle(
            999,
            "New description");

        Assert.False(result.Success);
        Assert.Null(result.Value);

        Assert.Equal(
            "Task #999 was not found.",
            result.Error);
    }

    [Fact]
    public void Update_ShouldRejectEmptyDescription()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "Original",
                Status = TaskStatus.Todo
            });

        var handler = new UpdateTaskHandler(store);

        var result = handler.Handle(
            1,
            "   ");

        Assert.False(result.Success);

        Assert.Equal(
            "Task description cannot be empty.",
            result.Error);
    }
}
