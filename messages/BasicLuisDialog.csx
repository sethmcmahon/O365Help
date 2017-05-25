using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
    private int quantity = 0;
    private string accountNumber = "";
    private string product = "";
    private string addOrRemove = "";

    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {
    }

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You have reached the none intent. You said: {result.Query}");
        
        context.Wait(MessageReceived);
    }

    [LuisIntent("ManageSubscriptions")]
    public async Task ManageSubscriptionsIntent(IDialogContext context, LuisResult result)
    {
        this.ClearManageSubscriptionState();

        if (result.Query.ToLower().Contains(" add") || result.Query.ToLower().Contains(" added"))
        {
            this.addOrRemove = "add";
        }
        if (result.Query.ToLower().Contains(" remove") || result.Query.ToLower().Contains(" removed"))
        {
            this.addOrRemove = "remove";
        }

        this.DisplayIntents(context, result);
        //this.DisplayEntities(context, result);

        foreach (EntityRecommendation  entity in result.Entities)
        {
            switch (entity.Type)
            {
                case "AccountNumber":
                    this.accountNumber = entity.Entity;
                    break;
                case "Product":
                    this.product = entity.Entity;
                    break;
                case "AddRemove":
                    this.addOrRemove = entity.Entity;
                    break;
                case "ProductQuantity":
                    Int32.TryParse(entity.Entity, out this.quantity);
                    break;
            }
        }

        if (this.accountNumber == "" || this.quantity == 0 || this.product == "")
        {
            await context.PostAsync("Sure, I can help you with that. I'll need to get a little more information.");
        }

        this.GetParms(context);
    }

    private async Task GetParms(IDialogContext context)
    {
        if (this.accountNumber == "")
        {
            context.Call<string>(new AccountNumberDialog(), this.AccountNumberDialogResumeAfter);
        }
        else if (this.product == "")
        {
            context.Call<string>(new ProductDialog(), this.ProductDialogResumeAfter);
        }
        else if (this.quantity == 0)
        {
            context.Call<int>(new QuantityDialog(), this.QuantityDialogResumeAfter);
        }
        else
        {
            await context.PostAsync($"All set. Your account {this.accountNumber}, now has {this.quantity} subscriptions of {this.product}.");
        }
    }
    
    private async Task QuantityDialogResumeAfter(IDialogContext context, IAwaitable<int> result)
    {
        try
        {
            this.quantity = await result;
            await context.PostAsync($"{this.quantity.ToString()}, got it!");
        }
        catch (TooManyAttemptsException)
        {
            await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
        }

        this.GetParms(context);
    }

    private async Task AccountNumberDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
    {
        try
        {
            this.accountNumber = await result;
            await context.PostAsync($"{this.accountNumber}, got it!");
        }
        catch (TooManyAttemptsException)
        {
            await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
        }

        this.GetParms(context);
    }

    private async Task ProductDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
    {
        try
        {
            this.product = await result;
            await context.PostAsync($"{this.product}, got it!");
        }
        catch (TooManyAttemptsException)
        {
            await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
        }

        this.GetParms(context);
    }

    private async Task DisplayIntents(IDialogContext context, LuisResult result)
    {
        foreach (IntentRecommendation  intent in result.Intents)
        {
            await context.PostAsync($"Intent: {intent.Intent}, Score: {intent.Score}");
        }
    }

    private async Task DisplayEntities(IDialogContext context, LuisResult result)
    {
        foreach (EntityRecommendation  entity in result.Entities)
        {
            await context.PostAsync($"Entity: {entity.Type}, Value: {entity.Entity}, Score: {entity.Score}");
        }
    }

    private void ClearManageSubscriptionState()
    {
        this.quantity = 0;
        this.accountNumber = "";
        this.product = "";
        this.addOrRemove = "";
    }
    
    [LuisIntent("EnableMailArchiving")]
    public async Task EnableMailArchivingIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Intent chosen: {result.TopScoringIntent.Intent}");
        
        this.DisplayIntents(context, result);
        this.DisplayEntities(context, result);
        
        context.Wait(MessageReceived);
    }
}
