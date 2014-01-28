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
using System.Text;
using System.Reflection;
using System.Threading;

namespace FrameWork
{
    public class ConsoleMgr
    {
        public readonly Dictionary<string, IConsoleHandler> m_consoleHandlers = new Dictionary<string,IConsoleHandler>();

        private static ConsoleMgr Instance = null;
        private bool _IsRunning = true;
        private DateTime _start_time = DateTime.Now;

        static public void Start()
        {
            if (Instance != null)
                return;

            Instance = new ConsoleMgr();

            try
            {
                Instance.LoadConsoleHandler();
            }
            catch (Exception e)
            {
                Log.Error("ConsoleMgr", "Can not load : " + e.ToString());
            }

            while (Instance._IsRunning)
            {
                string line;

                try
                {
                    line = Console.ReadLine();
                }
                catch
                {
                    // Console fermée
                    break;
                }

                if ( line == null )
                {
                    break;
                }

                    
                if (line.StartsWith("."))
                {
                    line = line.Substring(1);

                    if (!Instance.ExecuteCommand(line))
                        Log.Error("ConsoleMgr", "Command not found");
                }else CleanLine(line.Length);
            }
        }

        static public void CleanLine(int size)
        {
            string clear = new string(' ',size);
            Console.CursorLeft = 0;
            Console.CursorTop -= 1;
            Console.Write(clear);
            Console.CursorLeft = 0;
        }

        static public void Stop()
        {
            Instance._IsRunning = false;
        }

        static public void WaitAndExit(int WaitTime)
        {
            System.Threading.Thread.Sleep(WaitTime);
            Environment.Exit(0);
        }

        static public string GetUptime
        {
            get
            {
                DateTime Time = new DateTime(DateTime.Now.Ticks - Instance._start_time.Ticks);
                return Time.ToString("T");
            }
        }

        private void LoadConsoleHandler()
        {
            Log.Info("ConsoleMgr", "Commands list : ");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;

                    object[] attrib = type.GetCustomAttributes(typeof(ConsoleHandlerAttribute), true);
                    if (attrib.Length <= 0)
                        continue;

                    var consoleHandlerAttribs =
                        (ConsoleHandlerAttribute[])type.GetCustomAttributes(typeof(ConsoleHandlerAttribute), true);

                    if (consoleHandlerAttribs.Length > 0)
                    {
                        Log.Info("ConsoleMgr", "." + consoleHandlerAttribs[0].Command + " : " + consoleHandlerAttribs[0].Description);
                        RegisterHandler(consoleHandlerAttribs[0].Command, (IConsoleHandler)Activator.CreateInstance(type));
                    }
                }
            }
        }

        private void RegisterHandler(string command, IConsoleHandler Handler)
        {
            m_consoleHandlers.Add(command,Handler);
        }

        private bool ExecuteCommand(string line)
        {
            string command;
            List<string> args = new List<string>();

            int a = line.IndexOf(' ');

            if (a == -1)
                a = line.Length;

            command = line.Substring(0, a);
            line = line.Remove(0,a);

            if (command.Length <= 0)
                return false;
            
            IConsoleHandler Handler = null;

            if (!Instance.m_consoleHandlers.ContainsKey(command))
                return false;

            Handler = Instance.m_consoleHandlers[command];

            string[] Args = line.Split(' ');

            foreach (string str in Args)
                if( str.Length > 1 || (str.Length == 1 && str[0] != ' ') )
                    args.Add(str);

            var consoleHandlerAttribs = (ConsoleHandlerAttribute[])Handler.GetType().GetCustomAttributes(typeof(ConsoleHandlerAttribute), true);

            if (consoleHandlerAttribs[0].ArgCount != 0 && consoleHandlerAttribs[0].ArgCount > args.Count)
                Log.Error("ConsoleMgr", "Invalid parameter count : " + args.Count + " / " + consoleHandlerAttribs[0].ArgCount);
            else
            {
                try
                {
                    if (!Handler.HandleCommand(command, args))
                        Log.Error("ConsoleMgr", "Invalid command parameter !");
                }
                catch (Exception e)
                {
                    Log.Error("ConsoleMgr", e.ToString());
                }
            }

            return true;
        }

        static int GetInt(List<string> args)
        {
            if (args.Count <= 0)
                return -999;

           int result = int.Parse(args[0]);
           args.RemoveAt(0);

           return result;
        }

        static bool GetBool(List<string> args)
        {
            return GetInt(args) > 0 ? true : false;
        }

        static string GetTotalString(List<string> args, int num)
        {
            string Total = "";

            foreach (string str in args)
                Total = Total + " " + str;

            Total = Total.Remove(1);

            args.Clear();

            return Total;
        }

    }
}
