public sealed class AddTaskHandlerTests
{
    [Fact]
    public void Add_ShouldCreateTask()
    {
        var store = new InMemoryTaskStore();
        var handler = new AddTaskHandler(store);

        var result = handler.Handle("Learn C#");

        Assert.True(result.Success);
        Assert.NotNull(result.Value);

        Assert.Equal(1, result.Value.Id);
        Assert.Equal("Learn C#", result.Value.Description);
        Assert.Equal(TaskStatus.Todo, result.Value.Status);
    }

    [Fact]
    public void Add_ShouldTrimDescription()
    {
        var store = new InMemoryTaskStore();
        var handler = new AddTaskHandler(store);

        var result = handler.Handle("  Learn C#  ");

        Assert.True(result.Success);
        Assert.Equal(
            "Learn C#",
            result.Value!.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Add_ShouldRejectEmptyDescription(
        string description)
    {
        var store = new InMemoryTaskStore();
        var handler = new AddTaskHandler(store);

        var result = handler.Handle(description);

        Assert.False(result.Success);
        Assert.Null(result.Value);

        Assert.Equal(
            "Task description cannot be empty.",
            result.Error);
    }

    [Fact]
    public void Add_ShouldIncrementId()
    {
        var store = new InMemoryTaskStore();

        store.Seed(
            new TaskItem
            {
                Id = 1,
                Description = "First",
                Status = TaskStatus.Todo
            },
            new TaskItem
            {
                Id = 2,
                Description = "Second",
                Status = TaskStatus.Done
            });

        var handler = new AddTaskHandler(store);

        var result = handler.Handle("Third");

        Assert.True(result.Success);
        Assert.Equal(3, result.Value!.Id);
    }

    [Fact]
    public void Add_ShouldFail_WhenStoreCannotSave()
    {
        var store = new InMemoryTaskStore
        {
            SaveSucceeds = false
        };

        var handler = new AddTaskHandler(store);

        var result = handler.Handle("Learn C#");

        Assert.False(result.Success);
        Assert.Null(result.Value);

        Assert.Equal(
            "Could not save tasks.",
            result.Error);
    }
}
