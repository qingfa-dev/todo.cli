using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(List<TaskItem>))]
internal partial class AppJsonContext
    : JsonSerializerContext
{
}