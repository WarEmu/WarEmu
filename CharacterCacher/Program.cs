using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Common;

using MidgarFrameWork;
using MidgarFrameWork.Logger;

namespace CharacterCacher
{
    class Program
    {
        static public CharacterMgr CharMgr;

        [STAThread]
        static void Main(string[] args)
        {
            Log.Info("CharacterCacher", "Lancement");

            Assembly.Load("Common");

            if (!EasyServer.InitLog("CharacterCacher", "Configs/CharacterCacher.log"))
                return;

            if (!EasyServer.InitConfig("Configs/CharacterCacher.xml", "CharacterCacher"))
                return;

            if (!EasyServer.InitMysqlDB("Configs/CharacterCacher.db", "Characters"))
                return;

            if (!EasyServer.InitRpcServer("CharacterCacher",
                EasyServer.GetConfValue<string>("CharacterCacher", "CharacterCacher", "Key"),
                EasyServer.GetConfValue<int>("CharacterCacher", "CharacterCacher", "Port")))
                return;

            CharMgr = new CharacterMgr();
            CharacterMgr.Database = EasyServer.GetDatabase("Characters");

            CharMgr.LoadRealms();
            
            EasyServer.StartConsole();
        }
    }
}
