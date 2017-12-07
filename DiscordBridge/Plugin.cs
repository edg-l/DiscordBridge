using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using System.Reflection;
using TShockAPI.DB;
using MySql.Data.MySqlClient;

using DSharpPlus;
using System.Threading.Tasks;

namespace DiscordBridge
{
    [ApiVersion(2, 1)]
    public class DiscordBridge : TerrariaPlugin
    {
        #region Plugin Info
        public override string Name => "DiscordBridge";
        public override string Author => "Ryozuki";
        public override string Description => "A chat bridge between discord and terraria.";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        #endregion

        public Util.ConfigFile Config = new Util.ConfigFile();

        static DiscordClient DiscordBot;

        public DiscordBridge(Main game) : base(game)
        {
        }

        private void LoadConfig()
        {
            string path = Path.Combine(TShock.SavePath, "DiscordBridge.json");
            Config = Util.ConfigFile.Read(path);
        }

        public override async void Initialize()
        {
            LoadConfig();

            DiscordBot = new DiscordClient(new DiscordConfiguration
            {
                Token = Config.DiscordBotToken,
                TokenType = TokenType.Bot
            });

            DiscordBot.MessageCreated += DiscordBot_MessageCreated;

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
            TShockAPI.Hooks.PlayerHooks.PlayerChat += PlayerHooks_PlayerChat;

            await DiscordBot.ConnectAsync();
        }

        private async void PlayerHooks_PlayerChat(TShockAPI.Hooks.PlayerChatEventArgs e)
        {
            try
            {
                var channel = await DiscordBot.GetChannelAsync(Config.DiscordChannelID);
                await DiscordBot.SendMessageAsync(channel, e.TShockFormattedText);
            }
            catch(Exception a)
            {
                TShock.Log.ConsoleError("DiscordBridge error when sending message to discord: {0}", a.Message);
            }
            
        }

        private Task DiscordBot_MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (e.Author == DiscordBot.CurrentUser)
                return Task.CompletedTask;
            if (e.Channel.Id != Config.DiscordChannelID)
                return Task.CompletedTask;
            TShock.Utils.Broadcast(String.Format(Config.MessageFormatDiscordToTerraria, e.Author.Username, e.Message.Content), Config.GetColor());
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
            }
            base.Dispose(disposing);
        }

        #region Hooks
        private void OnInitialize(EventArgs args)
        {
            Util.DBHelper.Connect();

            Commands.ChatCommands.Add(new Command("DiscordBridge.reload".ToLower(), DiscordReload, "discord_reload")
            {
                HelpText = "Usage: /discord_reload"
            });
        }

        private void OnPostInitialize(EventArgs args)
        {

        }
        #endregion

        #region Commands
        private async void DiscordReload(CommandArgs args)
        {
            Config = Util.ConfigFile.Read(Path.Combine(TShock.SavePath, "DiscordBridge.json"));

            await DiscordBot.DisconnectAsync();

            DiscordBot = new DiscordClient(new DiscordConfiguration
            {
                Token = Config.DiscordBotToken,
                TokenType = TokenType.Bot
            });

            await DiscordBot.ConnectAsync();

            args.Player.SendSuccessMessage("Succesfully reloaded DiscordBridge.");
        }
        #endregion
    }
}
