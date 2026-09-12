public sealed record ListTasksResult(
    IReadOnlyList<TaskItem> Tasks,
    int TodoCount,
    int InProgressCount,
    int DoneCount);