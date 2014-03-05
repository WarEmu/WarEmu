/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Common;
using FrameWork;

namespace AccountCacher
{
    class Program
    {
        static public AccountMgr AcctMgr = null;
        static public AccountConfigs Config = null;
        static public RpcServer Server;

        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(onError);

            Log.Texte("", "-------------------- Account Cacher -------------------", ConsoleColor.DarkRed);
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
            Config = ConfigMgr.GetConfig<AccountConfigs>();

            // Loading log level from file
            if (!Log.InitLog(Config.LogLevel, "AccountCacher"))
                ConsoleMgr.WaitAndExit(2000);

            AccountMgr.Database = DBManager.Start(Config.AccountDB.Total(), ConnectionType.DATABASE_MYSQL, "Accounts");
            if (AccountMgr.Database == null)
                ConsoleMgr.WaitAndExit(2000);

            Server = new RpcServer(Config.RpcInfo.RpcClientStartingPort, 1);
            if (!Server.Start(Config.RpcInfo.RpcIp, Config.RpcInfo.RpcPort))
                ConsoleMgr.WaitAndExit(2000);

            AcctMgr = Server.GetLocalObject<AccountMgr>();
            AcctMgr.LoadRealms();

            ConsoleMgr.Start();
        }

        static void onError(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("OnError", e.ExceptionObject.ToString());
        }
    }
}
