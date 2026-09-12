using System.CommandLine;

public static class DeleteCommand
{
    public static Command Create(
        DeleteTaskHandler handler)
    {
        var command = new Command(
            "delete",
            "Delete a task.");

        var id = new Argument<int>("id")
        {
            Description = "The task ID."
        };

        command.Arguments.Add(id);

        command.SetAction(parseResult =>
        {
            var taskId = parseResult.GetValue(id);

            var result = handler.Handle(taskId);

            if (!result.Success)
            {
                return TaskConsole.Error(result.Error!);
            }

            TaskConsole.TaskDeleted(result.Value);

            return 0;
        });

        return command;
    }
}