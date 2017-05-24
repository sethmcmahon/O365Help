using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;

using Microsoft.Bot.Connector;

[Serializable]
public class ProductDialog : IDialog<string>
{
    private int attempts = 3;

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

        if (1=1)
        {
            context.Done<string>(product);
        }
        else
        {
            --attempts;
            if (attempts > 0)
            {
                await context.PostAsync("I'm sorry, I don't understand your reply. What is the product you would like to manage?");

                context.Wait(this.MessageReceivedAsync);
            }
            else
            {
                context.Fail(new TooManyAttemptsException("Message was not a valid product."));
            }
        }
    }
}