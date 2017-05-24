using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;

using Microsoft.Bot.Connector;

[Serializable]
public class ProductDialog : IDialog<string>
{
    public ProductDialog()
    {
    }

    public async Task StartAsync(IDialogContext context)
    {
        await context.PostAsync($"What product would you like to manage?");

        context.Wait(this.MessageReceivedAsync);
    }

    private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
    {
        string product = await result;
        context.Done<string>(product);
    }
}