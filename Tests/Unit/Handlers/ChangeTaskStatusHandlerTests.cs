public sealed class ChangeTaskStatusHandlerTests
{
    [Theory]
    [InlineData(TaskStatus.Todo)]
    [InlineData(TaskStatus.InProgress)]
    [InlineData(TaskStatus.Done)]
    public void ChangeStatus_ShouldUpdateStatus(
        string status)
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "Learn C#",
                Status = TaskStatus.Todo
            });

        var handler =
            new ChangeTaskStatusHandler(store);

        var result = handler.Handle(
            1,
            status);

        Assert.True(result.Success);
        Assert.Equal(
            status,
            result.Value!.Status);
    }

    [Fact]
    public void ChangeStatus_ShouldFail_WhenTaskDoesNotExist()
    {
        var store = new InMemoryTaskStore();
        var handler =
            new ChangeTaskStatusHandler(store);

        var result = handler.Handle(
            999,
            TaskStatus.Done);

        Assert.False(result.Success);
        Assert.Null(result.Value);

        Assert.Equal(
            "Task #999 was not found.",
            result.Error);
    }

    [Fact]
    public void ChangeStatus_ShouldRejectInvalidStatus()
    {
        var store = new InMemoryTaskStore();

        var handler =
            new ChangeTaskStatusHandler(store);

        var result = handler.Handle(
            1,
            "invalid-status");

        Assert.False(result.Success);
        Assert.Null(result.Value);

        Assert.Equal(
            "Unknown status: invalid-status",
            result.Error);
    }

    [Fact]
    public void ChangeStatus_ShouldFail_WhenStoreCannotSave()
    {
        var store = new InMemoryTaskStore
        {
            SaveSucceeds = false
        };

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "Learn C#",
                Status = TaskStatus.Todo
            });

        var handler =
            new ChangeTaskStatusHandler(store);

        var result = handler.Handle(
            1,
            TaskStatus.Done);

        Assert.False(result.Success);

        Assert.Equal(
            "Could not save tasks.",
            result.Error);
    }
}
