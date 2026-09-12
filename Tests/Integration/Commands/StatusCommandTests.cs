using System.Text.Json;

public sealed class StatusCommandTests
{
    [Fact]
    public async Task MarkDone_ShouldSetDoneStatus()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        var result = await cli.RunAsync(
            "mark-done",
            "1");

        Assert.Equal(0, result.ExitCode);

        var json =
            await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        Assert.Equal(
            TaskStatus.Done,
            document.RootElement[0]
                .GetProperty("Status")
                .GetString());
    }

    [Fact]
    public async Task MarkInProgress_ShouldSetInProgressStatus()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        var result = await cli.RunAsync(
            "mark-in-progress",
            "1");

        Assert.Equal(0, result.ExitCode);

        var json =
            await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        Assert.Equal(
            TaskStatus.InProgress,
            document.RootElement[0]
                .GetProperty("Status")
                .GetString());
    }

    [Fact]
    public async Task MarkDone_ShouldUpdateUpdatedAt()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        var beforeJson =
            await cli.ReadTasksJsonAsync();

        using var beforeDocument =
            JsonDocument.Parse(beforeJson);

        var before =
            beforeDocument.RootElement[0]
                .GetProperty("UpdatedAt")
                .GetDateTime();

        await cli.RunAsync(
            "mark-done",
            "1");

        var afterJson =
            await cli.ReadTasksJsonAsync();

        using var afterDocument =
            JsonDocument.Parse(afterJson);

        var after =
            afterDocument.RootElement[0]
                .GetProperty("UpdatedAt")
                .GetDateTime();

        Assert.True(after >= before);
    }

    [Fact]
    public async Task MarkInProgress_ShouldUpdateUpdatedAt()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        var beforeJson =
            await cli.ReadTasksJsonAsync();

        using var beforeDocument =
            JsonDocument.Parse(beforeJson);

        var before =
            beforeDocument.RootElement[0]
                .GetProperty("UpdatedAt")
                .GetDateTime();

        await cli.RunAsync(
            "mark-in-progress",
            "1");

        var afterJson =
            await cli.ReadTasksJsonAsync();

        using var afterDocument =
            JsonDocument.Parse(afterJson);

        var after =
            afterDocument.RootElement[0]
                .GetProperty("UpdatedAt")
                .GetDateTime();

        Assert.True(after >= before);
    }

    [Fact]
    public async Task MarkDone_ShouldBeVisibleInDoneList()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Finish this");

        await cli.RunAsync(
            "mark-done",
            "1");

        var result = await cli.RunAsync(
            "list",
            "done");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "Finish this",
            result.StdOut);
    }

    [Fact]
    public async Task MarkInProgress_ShouldBeVisibleInInProgressList()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Work on this");

        await cli.RunAsync(
            "mark-in-progress",
            "1");

        var result = await cli.RunAsync(
            "list",
            "in-progress");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "Work on this",
            result.StdOut);
    }

    [Fact]
    public async Task MarkDone_ShouldRemoveTaskFromTodoList()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Finish this");

        await cli.RunAsync(
            "mark-done",
            "1");

        var result = await cli.RunAsync(
            "list",
            "todo");

        Assert.Equal(0, result.ExitCode);

        Assert.DoesNotContain(
            "Finish this",
            result.StdOut);
    }

    [Fact]
    public async Task MarkInProgress_ShouldRemoveTaskFromTodoList()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Work on this");

        await cli.RunAsync(
            "mark-in-progress",
            "1");

        var result = await cli.RunAsync(
            "list",
            "todo");

        Assert.Equal(0, result.ExitCode);

        Assert.DoesNotContain(
            "Work on this",
            result.StdOut);
    }

    [Fact]
    public async Task StatusCommand_ShouldFailForMissingTask()
    {
        await using var cli = new CliTestHost();

        var doneResult = await cli.RunAsync(
            "mark-done",
            "999");

        Assert.NotEqual(0, doneResult.ExitCode);

        Assert.Contains(
            "Task #999 was not found",
            doneResult.StdOut);

        var progressResult = await cli.RunAsync(
            "mark-in-progress",
            "999");

        Assert.NotEqual(0, progressResult.ExitCode);

        Assert.Contains(
            "Task #999 was not found",
            progressResult.StdOut);
    }

    [Fact]
    public async Task Task_ShouldSupportMultipleStatusTransitions()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Learn C#");

        await cli.RunAsync(
            "mark-in-progress",
            "1");

        var progress =
            await cli.RunAsync(
                "list",
                "in-progress");

        Assert.Contains(
            "Learn C#",
            progress.StdOut);

        await cli.RunAsync(
            "mark-done",
            "1");

        var done =
            await cli.RunAsync(
                "list",
                "done");

        Assert.Contains(
            "Learn C#",
            done.StdOut);

        var todo =
            await cli.RunAsync(
                "list",
                "todo");

        Assert.DoesNotContain(
            "Learn C#",
            todo.StdOut);
    }
}
