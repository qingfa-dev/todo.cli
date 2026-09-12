using System.CommandLine;

public static class UpdateCommand
{
    public static Command Create(
        UpdateTaskHandler handler)
    {
        var command = new Command(
            "update",
            "Update a task description.");

        var id = new Argument<int>("id")
        {
            Description = "The task ID."
        };

        var description = new Argument<string>(
            "description")
        {
            Description = "The new task description."
        };

        command.Arguments.Add(id);
        command.Arguments.Add(description);

        command.SetAction(parseResult =>
        {
            var taskId = parseResult.GetValue(id);
            var value = parseResult.GetValue(description);

            var result = handler.Handle(
                taskId,
                value);

            if (!result.Success)
            {
                return TaskConsole.Error(result.Error!);
            }

            TaskConsole.TaskUpdated(result.Value!);

            return 0;
        });

        return command;
    }
}