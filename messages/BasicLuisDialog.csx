using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
    private int quantity;
    private string product = "Unknown";

    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {
    }

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You have reached the none intent. You said: {result.Query}");
        
        foreach (IntentRecommendation  intent in result.Intents)
        {
            await context.PostAsync($"{intent.Intent}, {intent.Score}"); //
        }

        context.Wait(MessageReceived);
    }

    [LuisIntent("ManageSubscriptions")]
    public async Task ManageSubscriptionsIntent(IDialogContext context, LuisResult result)
    {
        string accountNumber = "Unknown";
        string addOrRemove = "Unknown";

        if (result.Query.ToLower().Contains(" add") || result.Query.ToLower().Contains(" added"))
        {
            addOrRemove = "add";
        }
        if (result.Query.ToLower().Contains(" remove") || result.Query.ToLower().Contains(" removed"))
        {
            addOrRemove = "remove";
        }

        await context.PostAsync($"Intent chosen: {result.TopScoringIntent.Intent}");
        
        //foreach (IntentRecommendation  intent in result.Intents)
        //{
        //    await context.PostAsync($"Intent: {intent.Intent}, Score: {intent.Score}");
        //}

        foreach (EntityRecommendation  entity in result.Entities)
        {
            switch (entity.Type)
            {
                case "AccountNumber":
                    accountNumber = entity.Entity;
                    break;
                case "Product":
                    this.product = entity.Entity;
                    break;
                case "AddRemove":
                    addOrRemove = entity.Entity;
                    break;
                case "ProductQuantity":
                    Int32.TryParse(entity.Entity, out this.quantity);
                    break;
            }
            
            //await context.PostAsync($"Entity: {entity.Type}, Value: {entity.Entity}, Score: {entity.Score}");
        }

        this.GetParms(context);

        //await context.PostAsync($"Account Number: {accountNumber}, Quantity: {this.quantity}, Product: {this.product}, Add or Remove: {addOrRemove}");

        //context.Wait(MessageReceived);
    }

private GetParms(IDialogContext context)
{
    if (this.quantity == 0)
    {
        context.Call<int>(new QuantityDialog(), this.QuantityDialogResumeAfter);
    }
    else if (this.product == "Unknown")
    {
        //await context.PostAsync($"Quantity: {this.quantity}, Product: {this.product}");
        //context.Call<string>(new ProductDialog(), this.ProductDialogResumeAfter);
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
}
    
    [LuisIntent("EnableMailArchiving")]
    public async Task EnableMailArchivingIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Intent chosen: {result.TopScoringIntent.Intent}");
        
        foreach (IntentRecommendation  intent in result.Intents)
        {
            await context.PostAsync($"{intent.Intent}, {intent.Score}"); //
        }
        
        context.Wait(MessageReceived);
    }
}
