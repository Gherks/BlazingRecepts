using BlazingRecept.Shared.SerilogDto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Parsing;
using System.Text.Json;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/logs")]
public sealed class LogController : ControllerBase
{
    public LogController()
    {
        LogContext.PushProperty("Domain", "Log");
    }

    [HttpPost]
    public void PostAsync(JsonDocument logEventsJsonDocument)
    {
        if (logEventsJsonDocument == null)
        {
            string errorMessage = "Cannot process log events because log events json document is null.";
            Log.Error(errorMessage);
            throw new ArgumentNullException(nameof(logEventsJsonDocument), errorMessage);
        }

        string jsonString = logEventsJsonDocument.RootElement.GetRawText();

        if (jsonString == null)
        {
            string errorMessage = "Cannot process log events because content of log events json document is null.";
            Log.Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        LogEventDto[]? logEventDtos = JsonConvert.DeserializeObject<LogEventDto[]>(jsonString);

        if (logEventDtos == null)
        {
            string errorMessage = "Cannot process log events because deserialized log event dto list is null.";
            Log.Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        foreach (LogEventDto logEventDto in logEventDtos)
        {
            if (Enum.TryParse(logEventDto.Level, true, out LogEventLevel level))
            {
                bool hasExceptionMessage = string.IsNullOrWhiteSpace(logEventDto.Exception) == false;
                Exception? exception = hasExceptionMessage ? new Exception(logEventDto.Exception) : null;

                MessageTemplate messageTemplate = new MessageTemplateParser().Parse($"{logEventDto.RenderedMessage}");
                List<LogEventProperty> properties = BuildLogEventProperties(logEventDto);

                LogEvent logEvent = new LogEvent(logEventDto.Timestamp, level, exception, messageTemplate, properties);
                Log.Write(logEvent);
            }
            else
            {
                Log.Information($"ClientLog (Failed to determine log level) - {logEventDto.RenderedMessage}");
            }
        }
    }

    private List<LogEventProperty> BuildLogEventProperties(LogEventDto logEventDto)
    {
        List<LogEventProperty> properties = new();

        foreach (KeyValuePair<string, string> property in logEventDto.Properties)
        {
            properties.Add(new LogEventProperty(property.Key, new ScalarValue(property.Value)));
        }

        return properties;
    }
}
