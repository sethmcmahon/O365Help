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
        string product = "Unknown";
        string addOrRemove = "Unknown";
        int quantity = 0;
        string quantityValue = "Unknown";

        if (result.Query.ToLower().Contains(" add") || result.Query.ToLower().Contains(" added"))
        {
            addOrRemove = "add";
        }
        if (result.Query.ToLower().Contains(" remove") || result.Query.ToLower()Contains(" removed"))
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
                    product = entity.Entity;
                    break;
                case "AddRemove":
                    addOrRemove = entity.Entity;
                    break;
                case "ProductQuantity":
                    Int32.TryParse(entity.Entity, out quantity);
                    break;
            }
            
            //await context.PostAsync($"Entity: {entity.Type}, Value: {entity.Entity}, Score: {entity.Score}");
        }

        if (quantity != 0)
        {
            quantityValue = quantity.ToString();
        }

        await context.PostAsync($"Account Number: {accountNumber}, Quantity: {quantityValue}, Product: {product}, Add or Remove: {addOrRemove}");

        context.Wait(MessageReceived);
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
