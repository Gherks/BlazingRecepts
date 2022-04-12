namespace BlazingRecept.Shared.SerilogDto;

public sealed class LogEventDto
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.MinValue;
    public string Level { get; set; } = string.Empty;
    public string RenderedMessage { get; set; } = string.Empty;
    public string Exception { get; set; } = string.Empty;
    public Dictionary<string, string> Properties { get; set; } = new();
}
