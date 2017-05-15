using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;

    using Microsoft.Bot.Connector;

    [Serializable]
    public class QuantityDialog : IDialog<int>
    {
        private int attempts = 3;

        public QuantityDialog()
        {
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"What is new quantity?");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            int quantity;

            if (Int32.TryParse(message.Text, out quantity) && (quantity > 0))
            {
                context.Done(quantity);
            }
            else
            {
                --attempts;
                if (attempts > 0)
                {
                    await context.PostAsync("I'm sorry, I don't understand your reply. What is your quantity?");

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("Message was not a valid quantity."));
                }
            }
        }
    }
}