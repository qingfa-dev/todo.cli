using System.Text.Json;

public sealed class JsonTaskStore : ITaskStore
{
    private readonly string _dataFile;

    public JsonTaskStore(string dataFile = "tasks.json")
    {
        if (string.IsNullOrWhiteSpace(dataFile))
        {
            throw new ArgumentException(
                "Data file path cannot be empty.",
                nameof(dataFile));
        }

        _dataFile = dataFile;
    }

    public List<TaskItem> Load()
    {
        if (!File.Exists(_dataFile))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(_dataFile);

            if (string.IsNullOrWhiteSpace(json))
            {
                return [];
            }

            return JsonSerializer.Deserialize(
                       json,
                       AppJsonContext.Default.ListTaskItem)
                   ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
        catch (IOException)
        {
            return [];
        }
    }

    public bool Save(List<TaskItem> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        try
        {
            var directory = Path.GetDirectoryName(
                Path.GetFullPath(_dataFile));

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(
                tasks,
                AppJsonContext.Default.ListTaskItem);

            File.WriteAllText(_dataFile, json);

            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
