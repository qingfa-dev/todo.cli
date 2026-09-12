using System.CommandLine;

public static class MarkTodoCommand
{
    public static Command Create(
        ChangeTaskStatusHandler handler)
    {
        var command = new Command(
            "mark-todo",
            "Mark a task as todo.");

        var id = new Argument<int>("id")
        {
            Description = "The task ID."
        };

        command.Arguments.Add(id);

        command.SetAction(parseResult =>
        {
            var taskId = parseResult.GetValue(id);

            var result = handler.Handle(
                taskId,
                TaskStatus.Todo);

            if (!result.Success)
            {
                return TaskConsole.Error(
                    result.Error!);
            }

            TaskConsole.TaskStatusChanged(
                result.Value!);

            return 0;
        });

        return command;
    }
}