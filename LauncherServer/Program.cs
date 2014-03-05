
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Common;
using FrameWork;

namespace LauncherServer
{
    class Program
    {
        static public RpcClient Client = null;
        static public LauncherConfig Config = null;
        static public TCPServer Server = null;

        static public int Version
        {
            get
            {
                return Config.Version;
            }
        }
        static public string Message
        {
            get
            {
                return Config.Message;
            }
        }

        static public FileInfo Info;
        static public string StrInfo;

        static public AccountMgr AcctMgr
        {
            get
            {
                return Client.GetServerObject<AccountMgr>();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(onError);

            Log.Texte("", "------------------- Launcher Server -------------------", ConsoleColor.DarkRed);
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
            Config = ConfigMgr.GetConfig<LauncherConfig>();

            // Loading log level from file
            if (!Log.InitLog(Config.LogLevel, "LauncherServer"))
                ConsoleMgr.WaitAndExit(2000);

            Client = new RpcClient("LauncherServer", Config.RpcInfo.RpcLocalIp, 1);
            if (!Client.Start(Config.RpcInfo.RpcServerIp, Config.RpcInfo.RpcServerPort))
                ConsoleMgr.WaitAndExit(2000);

            Info = new FileInfo("Configs/mythloginserviceconfig.xml");
            if (!Info.Exists)
            {
                Log.Error("Configs/mythloginserviceconfig.xml", "Config file missing !");
                ConsoleMgr.WaitAndExit(5000);
            }

            StrInfo = Info.OpenText().ReadToEnd();

            if (!TCPManager.Listen<TCPServer>(Config.LauncherServerPort, "LauncherServer"))
                ConsoleMgr.WaitAndExit(2000);

            Server = TCPManager.GetTcp<TCPServer>("LauncherServer");

            ConsoleMgr.Start();
        }

        static void onError(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("OnError", e.ExceptionObject.ToString());
        }
    }
}
