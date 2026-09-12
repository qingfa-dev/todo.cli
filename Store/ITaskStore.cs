public interface ITaskStore
{
    List<TaskItem> Load();

    bool Save(List<TaskItem> tasks);
}