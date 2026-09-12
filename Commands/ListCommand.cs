using System.CommandLine;

public static class ListCommand
{
    public static Command Create(
        ListTasksHandler handler)
    {
        var command = new Command(
            "list",
            "List tasks.");

        var status = new Argument<string?>("status")
        {
            Description =
                "Optional status: todo, in-progress, or done.",
            Arity = ArgumentArity.ZeroOrOne
        };

        command.Arguments.Add(status);

        command.SetAction(parseResult =>
        {
            var value = parseResult.GetValue(status);

            string? normalized = null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                normalized = TaskStatus.Normalize(value!);

                if (normalized is null)
                {
                    return TaskConsole.Error(
                        "Status must be todo, in-progress, or done.");
                }
            }

            var result = handler.Handle(normalized);

            if (!result.Success)
            {
                return TaskConsole.Error(result.Error!);
            }

            TaskConsole.RenderTasks(result.Value!);

            return 0;
        });

        return command;
    }
}