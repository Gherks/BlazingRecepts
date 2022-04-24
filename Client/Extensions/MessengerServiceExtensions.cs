using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Serilog;

namespace BlazingRecept.Client.Extensions;

public static class MessengerServiceExtensions
{
    public static void AddSuccess(this IHxMessengerService? messengerService, string title, string message)
    {
        if (messengerService == null)
        {
            const string errorMessage = "Messenger service is not available during success toast creation.";
            Log.ForContext("Domain", "MessengerServiceExtensions").Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        messengerService.AddInformation(title, message);
    }
}
