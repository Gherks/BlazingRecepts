using BlazingRecept.Contract;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;

namespace BlazingRecept.Client.Extensions;

public static class MessengerServiceExtensions
{
    public static void AddSuccess(this IHxMessengerService? messengerService, string title, string message)
    {
        Contracts.LogAndThrowWhenNull(messengerService, "Messenger service is not available during success toast creation.");

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
