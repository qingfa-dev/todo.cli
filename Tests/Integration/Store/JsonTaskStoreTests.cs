public sealed class JsonTaskStoreTests : IDisposable
{
    private readonly string _tempDirectory;

    public JsonTaskStoreTests()
    {
        _tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "todo-cli-tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void Load_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        var store = CreateStore();

        var tasks = store.Load();

        Assert.Empty(tasks);
    }

    [Fact]
    public void Save_ShouldCreateFile()
    {
        var store = CreateStore();

        var tasks = new List<TaskItem>
        {
            CreateTask(1, "Learn C#")
        };

        var result = store.Save(tasks);

        Assert.True(result);
        Assert.True(File.Exists(GetFilePath()));
    }

    [Fact]
    public void SaveAndLoad_ShouldRoundTripTasks()
    {
        var store = CreateStore();

        var original = new List<TaskItem>
        {
            CreateTask(1, "Learn C#"),
            CreateTask(2, "Build CLI")
        };

        Assert.True(store.Save(original));

        var loaded = store.Load();

        Assert.Equal(2, loaded.Count);

        Assert.Equal(
            original[0].Id,
            loaded[0].Id);

        Assert.Equal(
            original[0].Description,
            loaded[0].Description);

        Assert.Equal(
            original[0].Status,
            loaded[0].Status);

        Assert.Equal(
            original[1].Id,
            loaded[1].Id);

        Assert.Equal(
            original[1].Description,
            loaded[1].Description);
    }

    [Fact]
    public void SaveAndLoad_ShouldPreserveStatus()
    {
        var store = CreateStore();

        var tasks = new List<TaskItem>
        {
            new()
            {
                Id = 1,
                Description = "Todo task",
                Status = TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                Description = "Progress task",
                Status = TaskStatus.InProgress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 3,
                Description = "Done task",
                Status = TaskStatus.Done,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        Assert.True(store.Save(tasks));

        var loaded = store.Load();

        Assert.Equal(
            TaskStatus.Todo,
            loaded[0].Status);

        Assert.Equal(
            TaskStatus.InProgress,
            loaded[1].Status);

        Assert.Equal(
            TaskStatus.Done,
            loaded[2].Status);
    }

    [Fact]
    public void SaveAndLoad_ShouldPreserveDates()
    {
        var store = CreateStore();

        var createdAt = new DateTime(
            2026, 9, 12, 10, 30, 0,
            DateTimeKind.Utc);

        var updatedAt = new DateTime(
            2026, 9, 12, 11, 30, 0,
            DateTimeKind.Utc);

        var task = new TaskItem
        {
            Id = 1,
            Description = "Test dates",
            Status = TaskStatus.Todo,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        Assert.True(store.Save([task]));

        var loaded = store.Load();

        Assert.Single(loaded);

        Assert.Equal(
            createdAt,
            loaded[0].CreatedAt);

        Assert.Equal(
            updatedAt,
            loaded[0].UpdatedAt);
    }

    [Fact]
    public void Load_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        File.WriteAllText(
            GetFilePath(),
            string.Empty);

        var store = CreateStore();

        var tasks = store.Load();

        Assert.Empty(tasks);
    }

    [Fact]
    public void Load_ShouldReturnEmptyList_WhenJsonIsInvalid()
    {
        File.WriteAllText(
            GetFilePath(),
            "{ invalid json");

        var store = CreateStore();

        var tasks = store.Load();

        Assert.Empty(tasks);
    }

    [Fact]
    public void Save_ShouldCreateParentDirectory()
    {
        var nestedDirectory = Path.Combine(
            _tempDirectory,
            "nested",
            "directory");

        var file = Path.Combine(
            nestedDirectory,
            "tasks.json");

        var store = new JsonTaskStore(file);

        var result = store.Save(
        [
            CreateTask(1, "Test")
        ]);

        Assert.True(result);
        Assert.True(File.Exists(file));
    }

    private JsonTaskStore CreateStore()
    {
        return new JsonTaskStore(
            GetFilePath());
    }

    private string GetFilePath()
    {
        return Path.Combine(
            _tempDirectory,
            "tasks.json");
    }

    private static TaskItem CreateTask(
        int id,
        string description)
    {
        var now = DateTime.UtcNow;

        return new TaskItem
        {
            Id = id,
            Description = description,
            Status = TaskStatus.Todo,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(
                _tempDirectory,
                recursive: true);
        }
    }
}
