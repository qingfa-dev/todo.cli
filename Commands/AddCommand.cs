using System.CommandLine;

public static class AddCommand
{
    public static Command Create(
        AddTaskHandler handler)
    {
        var command = new Command(
            "add",
            "Add a new task.");

        var description = new Argument<string>(
            "description")
        {
            Description = "The task description."
        };

        command.Arguments.Add(description);

        command.SetAction(parseResult =>
        {
            var value = parseResult.GetValue(description);

            var result = handler.Handle(value);

            if (!result.Success)
            {
                return TaskConsole.Error(result.Error!);
            }

            TaskConsole.TaskAdded(result.Value!);

            return 0;
        });

        return command;
    }
}