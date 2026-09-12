using System.Text.Json;

public sealed class ListCommandTests
{
    [Fact]
    public async Task List_ShouldShowAllTasks()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        await cli.RunAsync(
            "add",
            "Build CLI");

        var result = await cli.RunAsync(
            "list");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "Learn C#",
            result.StdOut);

        Assert.Contains(
            "Build CLI",
            result.StdOut);
    }

    [Fact]
    public async Task List_ShouldShowTasksInIdOrder()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "First");

        await cli.RunAsync(
            "add",
            "Second");

        await cli.RunAsync(
            "add",
            "Third");

        var result = await cli.RunAsync(
            "list");

        Assert.Equal(0, result.ExitCode);

        var first =
            result.StdOut.IndexOf("First", StringComparison.Ordinal);

        var second =
            result.StdOut.IndexOf("Second", StringComparison.Ordinal);

        var third =
            result.StdOut.IndexOf("Third", StringComparison.Ordinal);

        Assert.True(first >= 0);
        Assert.True(second > first);
        Assert.True(third > second);
    }

    [Fact]
    public async Task ListDone_ShouldOnlyShowDoneTasks()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        await cli.RunAsync(
            "add",
            "Build CLI");

        await cli.RunAsync(
            "mark-done",
            "1");

        var result = await cli.RunAsync(
            "list",
            "done");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "Learn C#",
            result.StdOut);

        Assert.DoesNotContain(
            "Build CLI",
            result.StdOut);
    }

    [Fact]
    public async Task ListTodo_ShouldOnlyShowTodoTasks()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        await cli.RunAsync(
            "add",
            "Build CLI");

        await cli.RunAsync(
            "mark-done",
            "1");

        var result = await cli.RunAsync(
            "list",
            "todo");

        Assert.Equal(0, result.ExitCode);

        Assert.DoesNotContain(
            "Learn C#",
            result.StdOut);

        Assert.Contains(
            "Build CLI",
            result.StdOut);
    }

    [Fact]
    public async Task ListInProgress_ShouldOnlyShowInProgressTasks()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        await cli.RunAsync(
            "add",
            "Build CLI");

        await cli.RunAsync(
            "mark-in-progress",
            "2");

        var result = await cli.RunAsync(
            "list",
            "in-progress");

        Assert.Equal(0, result.ExitCode);

        Assert.DoesNotContain(
            "Learn C#",
            result.StdOut);

        Assert.Contains(
            "Build CLI",
            result.StdOut);
    }

    [Fact]
    public async Task List_ShouldShowStatusCounts()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Todo task");

        await cli.RunAsync(
            "add",
            "Progress task");

        await cli.RunAsync(
            "add",
            "Done task");

        await cli.RunAsync(
            "mark-in-progress",
            "2");

        await cli.RunAsync(
            "mark-done",
            "3");

        var result = await cli.RunAsync(
            "list");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains("Todo: 1", result.StdOut);
        Assert.Contains("In progress: 1", result.StdOut);
        Assert.Contains("Done: 1", result.StdOut);
    }

    [Fact]
    public async Task List_ShouldShowNoTasksWhenEmpty()
    {
        await using var cli = new CliTestHost();

        var result = await cli.RunAsync(
            "list");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "No tasks found",
            result.StdOut);
    }

    [Fact]
    public async Task List_ShouldRejectInvalidStatus()
    {
        await using var cli = new CliTestHost();

        var result = await cli.RunAsync(
            "list",
            "invalid");

        Assert.NotEqual(0, result.ExitCode);

        Assert.Contains(
            "Status must be",
            result.StdOut);
    }

    [Fact]
    public async Task List_ShouldUsePersistedTasks()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Persistent task");

        var result = await cli.RunAsync(
            "list");

        Assert.Contains(
            "Persistent task",
            result.StdOut);

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        Assert.Single(
            document.RootElement.EnumerateArray());
    }
}
