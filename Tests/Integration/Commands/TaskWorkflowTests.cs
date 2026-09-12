public sealed class TaskWorkflowTests
{
    [Fact]
    public async Task CompleteTaskWorkflow_ShouldWork()
    {
        await using var cli = new CliTestHost();

        var add = await cli.RunAsync(
            "add",
            "Learn C#");

        Assert.Equal(
            0,
            add.ExitCode);

        var update = await cli.RunAsync(
            "update",
            "1",
            "Learn advanced C#");

        Assert.Equal(
            0,
            update.ExitCode);

        var markInProgress =
            await cli.RunAsync(
                "mark-in-progress",
                "1");

        Assert.Equal(
            0,
            markInProgress.ExitCode);

        var inProgress =
            await cli.RunAsync(
                "list",
                "in-progress");

        Assert.Contains(
            "Learn advanced C#",
            inProgress.StdOut);

        var markDone =
            await cli.RunAsync(
                "mark-done",
                "1");

        Assert.Equal(
            0,
            markDone.ExitCode);

        var done =
            await cli.RunAsync(
                "list",
                "done");

        Assert.Contains(
            "Learn advanced C#",
            done.StdOut);

        var todo =
            await cli.RunAsync(
                "list",
                "todo");

        Assert.DoesNotContain(
            "Learn advanced C#",
            todo.StdOut);

        var delete =
            await cli.RunAsync(
                "delete",
                "1");

        Assert.Equal(
            0,
            delete.ExitCode);

        var all =
            await cli.RunAsync(
                "list");

        Assert.Contains(
            "No tasks found",
            all.StdOut);
    }

    [Fact]
    public async Task MultipleTasks_ShouldMaintainIndependentState()
    {
        await using var cli = new CliTestHost();

        await cli.RunAsync(
            "add",
            "Task A");

        await cli.RunAsync(
            "add",
            "Task B");

        await cli.RunAsync(
            "add",
            "Task C");

        await cli.RunAsync(
            "mark-done",
            "1");

        await cli.RunAsync(
            "mark-in-progress",
            "2");

        var all =
            await cli.RunAsync(
                "list");

        Assert.Contains(
            "Task A",
            all.StdOut);

        Assert.Contains(
            "Task B",
            all.StdOut);

        Assert.Contains(
            "Task C",
            all.StdOut);

        var done =
            await cli.RunAsync(
                "list",
                "done");

        Assert.Contains(
            "Task A",
            done.StdOut);

        Assert.DoesNotContain(
            "Task B",
            done.StdOut);

        var progress =
            await cli.RunAsync(
                "list",
                "in-progress");

        Assert.Contains(
            "Task B",
            progress.StdOut);

        Assert.DoesNotContain(
            "Task A",
            progress.StdOut);

        var todo =
            await cli.RunAsync(
                "list",
                "todo");

        Assert.Contains(
            "Task C",
            todo.StdOut);

        Assert.DoesNotContain(
            "Task A",
            todo.StdOut);

        Assert.DoesNotContain(
            "Task B",
            todo.StdOut);
    }
}
