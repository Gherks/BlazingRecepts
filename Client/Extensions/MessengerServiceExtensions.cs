using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Serilog;

namespace BlazingRecept.Client.Extensions;

public static class MessengerServiceExtensions
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "MessengerServiceExtensions";

    public static void AddSuccess(this IHxMessengerService? messengerService, string title, string message)
    {
        if (messengerService == null)
        {
            const string errorMessage = "Messenger service is not available during success toast creation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        messengerService.AddMessage(new()
        {
            Icon = BootstrapIcon.HandThumbsUp,
            CssClass = "hx-messenger-success",
            AutohideDelay = 5000,
            Title = title,
            Text = message
        });
    }
}
