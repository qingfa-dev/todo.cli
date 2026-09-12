public sealed class DeleteCommandTests
{
    [Fact]
    public async Task Delete_ShouldRemoveTask()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Delete me");

        var result = await cli.RunAsync(
            "delete",
            "1");

        Assert.Equal(0, result.ExitCode);

        Assert.Contains(
            "Task #1 deleted",
            result.StdOut);

        var listResult = await cli.RunAsync(
            "list");

        Assert.Contains(
            "No tasks found",
            listResult.StdOut);
    }

    [Fact]
    public async Task Delete_ShouldOnlyRemoveSpecifiedTask()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Keep me");

        await cli.RunAsync(
            "add",
            "Delete me");

        await cli.RunAsync(
            "delete",
            "2");

        var result = await cli.RunAsync(
            "list");

        Assert.Contains(
            "Keep me",
            result.StdOut);

        Assert.DoesNotContain(
            "Delete me",
            result.StdOut);
    }

    [Fact]
    public async Task Delete_ShouldFailForMissingTask()
    {
        await using var cli = new CliTestHost();

        var result = await cli.RunAsync(
            "delete",
            "999");

        Assert.NotEqual(0, result.ExitCode);

        Assert.Contains(
            "Task #999 was not found",
            result.StdOut);
    }

    [Fact]
    public async Task Delete_ShouldLeaveOtherTaskIdsIntact()
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

        await cli.RunAsync(
            "delete",
            "2");

        var result = await cli.RunAsync(
            "list");

        Assert.Contains(
            "First",
            result.StdOut);

        Assert.DoesNotContain(
            "Second",
            result.StdOut);

        Assert.Contains(
            "Third",
            result.StdOut);
    }

    [Fact]
    public async Task Delete_ShouldUpdateJsonStorage()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Keep");

        await cli.RunAsync(
            "add",
            "Remove");

        await cli.RunAsync(
            "delete",
            "2");

        var json =
            await cli.ReadTasksJsonAsync();

        Assert.DoesNotContain(
            "Remove",
            json);

        Assert.Contains(
            "Keep",
            json);
    }
}
