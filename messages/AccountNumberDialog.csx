using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;

using Microsoft.Bot.Connector;

[Serializable]
public class AccountNumberDialog : IDialog<string>
{
    public AccountNumberDialog()
    {
    }

    public async Task StartAsync(IDialogContext context)
    {
        await context.PostAsync($"What is your account number?");

        context.Wait(this.MessageReceivedAsync);
    }

    private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
    {
        var message = await result;
        
        context.Done<string>(message.Text);
    }
}