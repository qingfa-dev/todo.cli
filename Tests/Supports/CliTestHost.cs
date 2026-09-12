using System.Diagnostics;

public sealed class CliTestHost : IAsyncDisposable
{
    private readonly string _workingDirectory;

    private static readonly SemaphoreSlim BuildLock = new(1, 1);

    private static string ProjectRoot =>
        FindProjectRoot();

    private static string AppOutputDirectory =>
        Path.Combine(ProjectRoot, "Tests", ".app");

    private static string AppDll =>
        Path.Combine(
            AppOutputDirectory,
            "task-cli.dll");

    public CliTestHost()
    {
        _workingDirectory = Path.Combine(
            Path.GetTempPath(),
            "task-cli-tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_workingDirectory);
    }

    public async Task<CliResult> RunAsync(
        params string[] arguments)
    {
        var cancellationToken =
            TestContext.Current.CancellationToken;

        await EnsureAppBuiltAsync(cancellationToken);

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = _workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add(AppDll);

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        var stdoutTask =
            process.StandardOutput.ReadToEndAsync(
                cancellationToken);

        var stderrTask =
            process.StandardError.ReadToEndAsync(
                cancellationToken);

        try
        {
            await process.WaitForExitAsync(
                cancellationToken);
        }
        catch
        {
            TryKill(process);
            throw;
        }

        return new CliResult(
            process.ExitCode,
            await stdoutTask,
            await stderrTask);
    }

    private static async Task EnsureAppBuiltAsync(
        CancellationToken cancellationToken)
    {
        if (File.Exists(AppDll))
        {
            return;
        }

        await BuildLock.WaitAsync(
            cancellationToken);

        try
        {
            if (File.Exists(AppDll))
            {
                return;
            }

            Directory.CreateDirectory(
                AppOutputDirectory);

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = ProjectRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("build");
            startInfo.ArgumentList.Add("task-cli.cs");
            startInfo.ArgumentList.Add("--output");
            startInfo.ArgumentList.Add(AppOutputDirectory);

            using var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            var stdoutTask =
                process.StandardOutput.ReadToEndAsync(
                    cancellationToken);

            var stderrTask =
                process.StandardError.ReadToEndAsync(
                    cancellationToken);

            try
            {
                await process.WaitForExitAsync(
                    cancellationToken);
            }
            catch
            {
                TryKill(process);
                throw;
            }

            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Failed to build task-cli.{Environment.NewLine}" +
                    $"STDOUT:{Environment.NewLine}{stdout}" +
                    $"STDERR:{Environment.NewLine}{stderr}");
            }
        }
        finally
        {
            BuildLock.Release();
        }
    }

    public string GetTasksFile()
    {
        return Path.Combine(
            _workingDirectory,
            "tasks.json");
    }

    public async Task<string> ReadTasksJsonAsync()
    {
        return await File.ReadAllTextAsync(
            GetTasksFile(),
            TestContext.Current.CancellationToken);
    }

    private static string FindProjectRoot()
    {
        var directory = new DirectoryInfo(
            AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                Path.Combine(
                    directory.FullName,
                    "task-cli.cs")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find project root containing task-cli.cs.");
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(
                    entireProcessTree: true);
            }
        }
        catch
        {
        }
    }

    public ValueTask DisposeAsync()
    {
        try
        {
            if (Directory.Exists(_workingDirectory))
            {
                Directory.Delete(
                    _workingDirectory,
                    recursive: true);
            }
        }
        catch
        {
        }

        return ValueTask.CompletedTask;
    }
}

public sealed record CliResult(
    int ExitCode,
    string StdOut,
    string StdErr);