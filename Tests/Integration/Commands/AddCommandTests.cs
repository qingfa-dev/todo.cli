using System.Text.Json;

public sealed class AddCommandTests
{
    [Fact]
    public async Task Add_ShouldCreateTask()
    {
        await using var cli = new CliTestHost();

        var result = await cli.RunAsync(
            "add",
            "Learn C#");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "Task #1 added",
            result.StdOut);

        Assert.True(
            File.Exists(cli.GetTasksFile()));
    }

    [Fact]
    public async Task Add_ShouldPersistTaskToJson()
    {
        await using var cli = new CliTestHost();

        var result = await cli.RunAsync(
            "add",
            "Learn C#");

        Assert.Equal(0, result.ExitCode);

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        var tasks = document.RootElement
            .EnumerateArray()
            .ToArray();

        Assert.Single(tasks);

        var task = tasks[0];

        Assert.Equal(
            1,
            task.GetProperty("Id").GetInt32());

        Assert.Equal(
            "Learn C#",
            task.GetProperty("Description").GetString());

        Assert.Equal(
            TaskStatus.Todo,
            task.GetProperty("Status").GetString());
    }

    [Fact]
    public async Task Add_ShouldSetCreatedAt()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        var createdAt =
            document.RootElement[0]
                .GetProperty("CreatedAt")
                .GetDateTime();

        Assert.NotEqual(
            default,
            createdAt);
    }

    [Fact]
    public async Task Add_ShouldSetUpdatedAt()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        var updatedAt =
            document.RootElement[0]
                .GetProperty("UpdatedAt")
                .GetDateTime();

        Assert.NotEqual(
            default,
            updatedAt);
    }

    [Fact]
    public async Task Add_ShouldSetCreatedAtAndUpdatedAtConsistently()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        var task = document.RootElement[0];

        var createdAt =
            task.GetProperty("CreatedAt")
                .GetDateTime();

        var updatedAt =
            task.GetProperty("UpdatedAt")
                .GetDateTime();

        Assert.Equal(
            createdAt,
            updatedAt);
    }

    [Fact]
    public async Task Add_ShouldTrimDescription()
    {
        await using var cli = new CliTestHost();

        var result = await cli.RunAsync(
            "add",
            "  Learn C#  ");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "Learn C#",
            result.StdOut);

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        Assert.Equal(
            "Learn C#",
            document.RootElement[0]
                .GetProperty("Description")
                .GetString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task Add_ShouldRejectEmptyDescription(
        string description)
    {
        await using var cli = new CliTestHost();

        var result = await cli.RunAsync(
            "add",
            description);

        Assert.NotEqual(
            0,
            result.ExitCode);

        Assert.Contains(
            "Task description cannot be empty",
            result.StdOut);

        Assert.False(
            File.Exists(cli.GetTasksFile()));
    }

    [Fact]
    public async Task Add_ShouldAssignIncrementingIds()
    {
        await using var cli = new CliTestHost();

        var first = await cli.RunAsync(
            "add",
            "First");

        var second = await cli.RunAsync(
            "add",
            "Second");

        var third = await cli.RunAsync(
            "add",
            "Third");

        Assert.Equal(
            0,
            first.ExitCode);

        Assert.Equal(
            0,
            second.ExitCode);

        Assert.Equal(
            0,
            third.ExitCode);

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        var tasks = document.RootElement
            .EnumerateArray()
            .ToArray();

        Assert.Equal(
            3,
            tasks.Length);

        Assert.Equal(
            1,
            tasks[0].GetProperty("Id").GetInt32());

        Assert.Equal(
            2,
            tasks[1].GetProperty("Id").GetInt32());

        Assert.Equal(
            3,
            tasks[2].GetProperty("Id").GetInt32());
    }

    [Fact]
    public async Task Add_ShouldCreateTasksIndependently()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        await cli.RunAsync(
            "add",
            "Build CLI");

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        var tasks = document.RootElement
            .EnumerateArray()
            .ToArray();

        Assert.Equal(
            "Learn C#",
            tasks[0]
                .GetProperty("Description")
                .GetString());

        Assert.Equal(
            "Build CLI",
            tasks[1]
                .GetProperty("Description")
                .GetString());

        Assert.All(
            tasks,
            task => Assert.Equal(
                TaskStatus.Todo,
                task.GetProperty("Status").GetString()));
    }

    [Fact]
    public async Task Add_ShouldBeVisibleInList()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        var result = await cli.RunAsync(
            "list");

        Assert.Equal(
            0,
            result.ExitCode);

        Assert.Contains(
            "Learn C#",
            result.StdOut);

        Assert.Contains(
            "TODO",
            result.StdOut);
    }
}