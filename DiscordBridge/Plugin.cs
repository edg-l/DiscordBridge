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

namespace DiscordBridge
{
    [ApiVersion(2, 1)]
    public class DiscordBridge : TerrariaPlugin
    {
        #region Plugin Info
        public override string Name => "DiscordBridge";
        public override string Author => "Ryozuki";
        public override string Description => "I do this and that.";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        #endregion

        public Util.ConfigFile Config = new Util.ConfigFile();

        public DiscordBridge(Main game) : base(game)
        {
        }

        private void LoadConfig()
        {
            string path = Path.Combine(TShock.SavePath, "DiscordBridge.json");
            Config = Util.ConfigFile.Read(path);
        }

        public override void Initialize()
        {
            LoadConfig();
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
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
            /*Util.DBHelper.CreateTable(new SqlTable("DiscordBridge",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, Unique = true, Length = 7, AutoIncrement = true },
                new SqlColumn("UserID", MySqlDbType.Int32) { Length = 6 },
                new SqlColumn("Balance", MySqlDbType.Float) { DefaultValue = "0" }
                ));*/

            Commands.ChatCommands.Add(new Command("DiscordBridge.help".ToLower(), CHelp, "chelp")
            {
                HelpText = "Usage: /chelp"
            });
        }

        private void OnPostInitialize(EventArgs args)
        {

        }
        #endregion

        #region Commands
        private void CHelp(CommandArgs args)
        {
            args.Player.SendInfoMessage("Author, please change me.");
        }
        #endregion
    }
}
