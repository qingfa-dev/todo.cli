using System.Text.Json;

public sealed class UpdateCommandTests
{
    [Fact]
    public async Task Update_ShouldChangeDescription()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Original");

        var result = await cli.RunAsync(
            "update",
            "1",
            "Updated");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "Task #1 updated",
            result.StdOut);

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        Assert.Equal(
            "Updated",
            document.RootElement[0]
                .GetProperty("Description")
                .GetString());
    }

    [Fact]
    public async Task Update_ShouldTrimDescription()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Original");

        var result = await cli.RunAsync(
            "update",
            "1",
            "  Updated  ");

        Assert.Equal(0, result.ExitCode);

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        Assert.Equal(
            "Updated",
            document.RootElement[0]
                .GetProperty("Description")
                .GetString());
    }

    [Fact]
    public async Task Update_ShouldPreserveTaskId()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Original");

        await cli.RunAsync(
            "update",
            "1",
            "Updated");

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        Assert.Equal(
            1,
            document.RootElement[0]
                .GetProperty("Id")
                .GetInt32());
    }

    [Fact]
    public async Task Update_ShouldPreserveStatus()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Original");

        await cli.RunAsync(
            "mark-in-progress",
            "1");

        await cli.RunAsync(
            "update",
            "1",
            "Updated");

        var json = await cli.ReadTasksJsonAsync();

        using var document =
            JsonDocument.Parse(json);

        Assert.Equal(
            TaskStatus.InProgress,
            document.RootElement[0]
                .GetProperty("Status")
                .GetString());
    }

    [Fact]
    public async Task Update_ShouldFailForMissingTask()
    {
        await using var cli = new CliTestHost();

        var result = await cli.RunAsync(
            "update",
            "999",
            "Updated");

        Assert.NotEqual(0, result.ExitCode);

        Assert.Contains(
            "Task #999 was not found",
            result.StdOut);
    }

    [Fact]
    public async Task Update_ShouldRejectEmptyDescription()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Original");

        var result = await cli.RunAsync(
            "update",
            "1",
            "   ");

        Assert.NotEqual(0, result.ExitCode);

        Assert.Contains(
            "Task description cannot be empty",
            result.StdOut);
    }

    [Fact]
    public async Task Update_ShouldChangeUpdatedAt()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Original");

        var beforeJson =
            await cli.ReadTasksJsonAsync();

        using var beforeDocument =
            JsonDocument.Parse(beforeJson);

        var before =
            beforeDocument.RootElement[0]
                .GetProperty("UpdatedAt")
                .GetDateTime();

        await cli.RunAsync(
            "update",
            "1",
            "Updated");

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
}
