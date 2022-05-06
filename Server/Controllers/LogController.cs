using BlazingRecept.Shared;
using BlazingRecept.Shared.SerilogDto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using System.Text.Json;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/logs")]
public sealed class LogController : ControllerBase
{
    [HttpPost]
    public void PostAsync(JsonDocument logEventsJsonDocument)
    {
        string jsonString = logEventsJsonDocument.RootElement.GetRawText();

        Contracts.LogAndThrowWhenNull(jsonString, "Cannot process log events because content of log events json document is null.");

        LogEventDto[]? logEventDtos = JsonConvert.DeserializeObject<LogEventDto[]>(jsonString);

        Contracts.LogAndThrowWhenNull(logEventDtos, "Cannot process log events because deserialized log event dto list is null.");

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
