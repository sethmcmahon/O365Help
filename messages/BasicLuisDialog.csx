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
        await context.PostAsync($"Here are the LUIS intents: ");

        //retrieve list of intents
        var intents = result.intents;
        foreach (var intent in intents)
        {
            await context.PostAsync($"{intent.Intent}, {intent.Score}"); //
        }

        context.Wait(MessageReceived);
    }

    [LuisIntent("ManageSubscriptions")]
    public async Task ManageSubscriptionsIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You have reached the ManageSubscriptions intent. You said: {result.Query}"); //
        context.Wait(MessageReceived);
    }
    
    [LuisIntent("EnableMailArchiving")]
    public async Task EnableMailArchivingIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You have reached the EnableMailArchiving intent. You said: {result.Query}"); //
        context.Wait(MessageReceived);
    }
}