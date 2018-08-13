# DiscordAPI for Fougerite

## Discord API
You will need to setup yourself a bot if you would like to configure yourself the discord api.
I suggest reading: [Here](https://discordapp.com/developers/docs/reference)

## Configuration
* BotToken: This is the bot token you get when setting up your bot as part of your discord server.
I suggest reading: [Here](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token)
* ChannelID: This is the ID of the channel you want to send the messsages to. Enable developer mode on your discord client, 
Right on the channel and select "Get ChannelID". This will copy the ChannelID to your clipboard and you can paste it in the configuration file.

## Example Use
```
    public void Blabla(string mymessage)
    {
        DiscordAPI.DiscordAPIInstance.SendMessageToBot(mymessage, MyCallback);
    }

    public void MyCallback(string receiveddata) 
    {
    
    }
```