using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Schema;
using Fougerite;

namespace DiscordAPI
{
    public class DiscordAPI : Fougerite.Module
    {
        public IniParser Settings;
        public string DiscordURL = "https://discordapp.com/api/channels/%ChannelID%/messages";
        private static DiscordAPI _instance;
        public int ChannelID;
        public string BotToken;
        
        [Flags]
        public enum MySecurityProtocolType
        {
            //
            // Summary:
            //     Specifies the Secure Socket Layer (SSL) 3.0 security protocol.
            Ssl3 = 48,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.0 security protocol.
            Tls = 192,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.1 security protocol.
            Tls11 = 768,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.2 security protocol.
            Tls12 = 3072
        }
        
        public override string Name
        {
            get { return "DiscordAPI"; }
        }

        public override string Author
        {
            get { return "DreTaX"; }
        }

        public override string Description
        {
            get { return "DiscordAPI"; }
        }

        public override Version Version
        {
            get { return new Version("1.0"); }
        }

        public override void Initialize()
        {
            _instance = this;
            ReloadConfig();
            
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.DefaultConnectionLimit = 200;
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(MySecurityProtocolType.Tls12 | MySecurityProtocolType.Tls11 | MySecurityProtocolType.Tls);
        }

        public override void DeInitialize()
        {

        }

        public static DiscordAPI DiscordAPIInstance
        {
            get { return _instance; }
        }

        /// <summary>
        /// This is what you should call to get an async callback.
        /// </summary>
        /// <param name="message">The message you want to send accordingly with the discord api.</param>
        /// <param name="callback">Your Method's name</param>
        public void SendMessageToBot(string message, Action<string> callback, Dictionary<string, string> AdditionalHeaders = null)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(DiscordURL);
            request.Headers["Authorization"] =
                BotToken.StartsWith("Bot ") ? BotToken : string.Format("Bot {0}", BotToken);
            request.Headers["Content-Type"] = "application/json";
            request.Method = "GET"; // May requires changing?
            if (AdditionalHeaders != null)
            {
                foreach (var x in AdditionalHeaders.Keys)
                {
                    request.Headers[x] = AdditionalHeaders[x];
                }
            }
            
            DoWithResponse(request, (response) =>
            {
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    string body = new StreamReader(stream).ReadToEnd();
                    callback(body);
                }
                else
                {
                    callback("Failed");
                }
            });
        }
        
        private void DoWithResponse(HttpWebRequest request, Action<HttpWebResponse> responseAction)
        {
            Action wrapperAction = () =>
            {
                request.BeginGetResponse(new AsyncCallback((iar) =>
                {
                    var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                    responseAction(response);
                }), request);
            };
            wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
            {
                var action = (Action)iar.AsyncState;
                action.EndInvoke(iar);
            }), wrapperAction);
        }

        public void ReloadConfig()
        {
            DiscordURL = "https://discordapp.com/api/channels/%ChannelID%/messages";
            try
            {
                if (!File.Exists(ModuleFolder + "\\Settings.ini"))
                {
                    File.Create(ModuleFolder + "\\Settings.ini").Dispose();
                    Settings = new IniParser(ModuleFolder + "\\Settings.ini");
                    Settings.AddSetting("Settings", "BotToken", "Bot INSERTTOKENHERE");
                    Settings.AddSetting("Settings", "ChannelID", "666");
                    Settings.Save();
                }
                Settings = new IniParser(ModuleFolder + "\\Settings.ini");
                BotToken = Settings.GetSetting("Settings", "BotToken");
                ChannelID = int.Parse(Settings.GetSetting("Settings", "ChannelID"));
                
            }
            catch (Exception ex)
            {
                Logger.LogError("[DiscordAPI] Config failed: " + ex);
            }
            DiscordURL = DiscordURL.Replace("%ChannelID%", ChannelID.ToString());
        }
    }
}