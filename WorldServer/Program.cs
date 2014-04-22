using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

using Common;
using FrameWork;

namespace WorldServer
{
    class Program
    {
        static public WorldConfigs Config = null;
        static public RpcClient Client = null;
        static public AccountMgr AcctMgr
        {
            get
            {
                return Client.GetServerObject<AccountMgr>();
            }
        }
        static public TCPServer Server;
        static public Realm Rm;

        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(onError);
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnClose);

            Log.Texte("", "-------------------- World Server ---------------------", ConsoleColor.DarkRed);
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
            Config = ConfigMgr.GetConfig<WorldConfigs>();

            // Loading log level from file
            if (!Log.InitLog(Config.LogLevel, "WorldServer"))
                ConsoleMgr.WaitAndExit(2000);

            CharMgr.Database = DBManager.Start(Config.CharacterDatabase.Total(), ConnectionType.DATABASE_MYSQL, "Characters");
            if (CharMgr.Database == null)
                ConsoleMgr.WaitAndExit(2000);

            WorldMgr.Database = DBManager.Start(Config.WorldDatabase.Total(), ConnectionType.DATABASE_MYSQL, "World");
            if (WorldMgr.Database == null)
                ConsoleMgr.WaitAndExit(2000);

            AbilityMgr.Database = WorldMgr.Database;

            Client = new RpcClient("WorldServer-" + Config.RealmId, Config.AccountCacherInfo.RpcLocalIp, 1);
            if (!Client.Start(Config.AccountCacherInfo.RpcServerIp, Config.AccountCacherInfo.RpcServerPort))
                ConsoleMgr.WaitAndExit(2000);
            

            Rm = Program.AcctMgr.GetRealm(Config.RealmId);

            if (Rm == null)
            {
                Log.Error("WorldServer", "Realm (" + Config.RealmId + ") not found");
                return;
            }

            LoaderMgr.Start();

            if (!TCPManager.Listen<TCPServer>(Rm.Port, "World"))
                ConsoleMgr.WaitAndExit(2000);

            Server = TCPManager.GetTcp<TCPServer>("World");

            AcctMgr.UpdateRealm(Client.Info, Rm.RealmId);
            AcctMgr.UpdateRealmCharacters(Rm.RealmId, (uint)CharMgr.Database.GetObjectCount<Character>("Realm=1"), (uint)CharMgr.Database.GetObjectCount<Character>("Realm=2"));

            ConsoleMgr.Start();
        }

        static void onError(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("onError", e.ExceptionObject.ToString());
        }

        static public void OnClose(object obj, object Args)
        {
            Log.Info("Fermeture", "Fermeture du serveur");

            WorldMgr.Stop();
            Player.Stop();
        }
    }
}
