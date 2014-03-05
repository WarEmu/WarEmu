
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace LobbyServer
{
    class Program
    {
        static public LobbyConfigs Config = null;

        static public RpcClient Client = null;
        static public TCPServer Server = null;

        static public AccountMgr AcctMgr
        {
            get
            {
                return Client.GetServerObject<AccountMgr>();
            }
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(onError);

            Log.Texte("", "-------------------- Lobby Server ---------------------", ConsoleColor.DarkRed);
            Log.Texte("", " █     █░ ▄▄▄       ██▀███  ▓█████  ███▄ ▄███▓ █    ██ ", ConsoleColor.Red);
            Log.Texte("", "▓█░ █ ░█░▒████▄    ▓██ ▒ ██▒▓█   ▀ ▓██▒▀█▀ ██▒ ██  ▓██▒", ConsoleColor.Red);
            Log.Texte("", "▒█░ █ ░█ ▒██  ▀█▄  ▓██ ░▄█ ▒▒███   ▓██    ▓██░▓██  ▒██░", ConsoleColor.Red);
            Log.Texte("", "░█░ █ ░█ ░██▄▄▄▄██ ▒██▀▀█▄  ▒▓█  ▄ ▒██    ▒██ ▓▓█  ░██░", ConsoleColor.Red);
            Log.Texte("", "░░██▒██▓  ▓█   ▓██▒░██▓ ▒██▒░▒████▒▒██▒   ░██▒▒▒█████▓ ", ConsoleColor.Red);
            Log.Texte("", "░ ▓░▒ ▒   ▒▒   ▓▒█░░ ▒▓ ░▒▓░░░ ▒░ ░░ ▒░   ░  ░░▒▓▒ ▒ ▒ ", ConsoleColor.Red);
            Log.Texte("", "  ▒ ░ ░    ▒   ▒▒ ░  ░▒ ░ ▒░ ░ ░  ░░  ░      ░░░▒░ ░ ░ ", ConsoleColor.Red);
            Log.Texte("", "  ░   ░    ░   ▒     ░░   ░    ░   ░      ░    ░░░ ░ ░ ", ConsoleColor.Red);
            Log.Texte("", "    ░          ░  ░   ░        ░  ░       ░      ░    ", ConsoleColor.Red);
            Log.Texte("", "-------------------http://WarEmu.com-------------------", ConsoleColor.DarkRed);

            // Loading all configs files
            ConfigMgr.LoadConfigs();
            Config = ConfigMgr.GetConfig<LobbyConfigs>();

            // Loading log level from file
            if (!Log.InitLog(Config.LogLevel, "LobbyServer"))
                ConsoleMgr.WaitAndExit(2000);

            Client = new RpcClient("LobbyServer", Config.RpcInfo.RpcLocalIp, 1);
            if (!Client.Start(Config.RpcInfo.RpcServerIp, Config.RpcInfo.RpcServerPort))
                ConsoleMgr.WaitAndExit(2000);

            if (!TCPManager.Listen<TCPServer>(Config.ClientPort, "LobbyServer"))
                ConsoleMgr.WaitAndExit(2000);

            Server = TCPManager.GetTcp<TCPServer>("LobbyServer");

            ConsoleMgr.Start();
        }

        static void onError(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("onError", e.ExceptionObject.ToString());
        }
    }
}
