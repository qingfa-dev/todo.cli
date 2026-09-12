using Spectre.Console;

public static class TaskConsole
{
    public static int Error(string message)
    {
        AnsiConsole.MarkupLine(
            $"[red]Error:[/] {Markup.Escape(message)}");

        return 1;
    }

    public static void TaskAdded(TaskItem task)
    {
        AnsiConsole.MarkupLine(
            $"[green]✓[/] Task [bold]#{task.Id}[/] added: " +
            $"[cyan]{Markup.Escape(task.Description)}[/]");
    }

    public static void TaskUpdated(TaskItem task)
    {
        AnsiConsole.MarkupLine(
            $"[green]✓[/] Task [bold]#{task.Id}[/] updated.");
    }

    public static void TaskDeleted(int id)
    {
        AnsiConsole.MarkupLine(
            $"[green]✓[/] Task [bold]#{id}[/] deleted.");
    }

    public static void TaskStatusChanged(TaskItem task)
    {
        AnsiConsole.MarkupLine(
            $"[green]✓[/] Task [bold]#{task.Id}[/] " +
            $"marked as {FormatStatus(task.Status)}.");
    }

    public static void RenderTasks(ListTasksResult result)
    {
        if (result.Tasks.Count == 0)
        {
            AnsiConsole.MarkupLine(
                "[yellow]No tasks found.[/]");

            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Task Tracker[/]");

        table.AddColumn(
            new TableColumn("[bold]ID[/]")
                .Centered());

        table.AddColumn("[bold]Status[/]");
        table.AddColumn("[bold]Description[/]");
        table.AddColumn("[bold]Created[/]");

        foreach (var task in result.Tasks)
        {
            table.AddRow(
                task.Id.ToString(),
                FormatStatus(task.Status),
                Markup.Escape(task.Description),
                task.CreatedAt
                    .ToLocalTime()
                    .ToString("yyyy-MM-dd HH:mm"));
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine(
            $"[blue]Todo:[/] {result.TodoCount}  " +
            $"[yellow]In progress:[/] {result.InProgressCount}  " +
            $"[green]Done:[/] {result.DoneCount}");
    }

    private static string FormatStatus(string status)
    {
        return status switch
        {
            TaskStatus.Todo => "[blue]TODO[/]",
            TaskStatus.InProgress => "[yellow]IN-PROGRESS[/]",
            TaskStatus.Done => "[green]DONE[/]",
            _ => "[grey]UNKNOWN[/]"
        };
    }
}