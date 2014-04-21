/*
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
using System.IO;

namespace FrameWork
{
    static public class Log
    {
        static private LogConfig _config = new LogConfig();
        static public LogConfig Config
        {
            get
            {
                return _config;
            }
        }

        static private FileInfo DumpFile = null;
        static private FileStream FSDump = null;

        static public void Init(LogConfig Config)
        {
            InitInstance(Config);
        }

        static public bool InitLog(string LogConf, string PrefileName)
        {
            try
            {
                LogConfig Conf = new LogConfig(0);
                Conf.PreFileName = PrefileName;

                if (LogConf.Length > 0)
                    Conf.LoadInfoFromFile(LogConf);

                Log.Init(Conf);
            }
            catch (Exception e)
            {
                Log.Error("InitLog", "Error : " + e.ToString());
                return false;
            }

            Log.Notice("InitLog", "Logger initialized");
            return true;
        }

        static public bool InitLog(LogInfo Info, string PrefileName)
        {
            LogConfig Config = new LogConfig(Info);
            Config.PreFileName = PrefileName;
            Log.Init(Config);
            return true;
        }

        static public void InitInstance(LogConfig Config)
        {
            try
            {
                if (Config == null)
                    Config = new LogConfig();

                string FileDir = Directory.GetCurrentDirectory() + Config.LogFolder;
                string BackDir = Directory.GetCurrentDirectory() + Config.LogFolder + "/Backup";

                try
                {
                    Directory.CreateDirectory(FileDir);
                    Directory.CreateDirectory(BackDir);
                }
                catch (Exception)
                {

                }

                FileDir += "/" + Config.PreFileName + Config.FileName;
                BackDir += "/" + Config.PreFileName + "." + DateTime.Now.Hour + "h." + DateTime.Now.Minute + "m." + DateTime.Now.Second + "s" + Config.FileName;

                if (DumpFile == null)
                {
                    DumpFile = new FileInfo(FileDir);
                    if (DumpFile.Exists)
                        DumpFile.MoveTo(BackDir);

                    DumpFile = new FileInfo(FileDir);

                    if (FSDump != null)
                        FSDump.Close();

                    FSDump = DumpFile.Create();
                }

                if (Config != null)
                    _config = Config;

            }
            catch (Exception)
            {
                Console.WriteLine("Log : Log file already in use.");

                if (Config != null)
                    Config.Info.Dump = false;
            }
        }

        public static void Texte(string name, string message, ConsoleColor Color)
        {
            lock (_config)
            {
                string Texte = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + name + " : " + message;

                Type isMono = Type.GetType("Mono.Runtime");
                if (isMono == null)
                    Console.BufferHeight = Console.WindowWidth - 20;

                Console.ForegroundColor = Color;
                Console.WriteLine(Texte);
                Console.ForegroundColor = ConsoleColor.White;

                if (DumpFile != null && FSDump != null)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(Texte+"\n");
                    FSDump.Write(info, 0, info.Length);
                    FSDump.Flush();
                }

            }
        }

        public static void Enter() // Saute une ligne
        {
            lock (_config)
            {
                Console.WriteLine("");
            }
        }

        public static void Info(string name,string message)
        {
            if (_config.Info.Info)
                Texte("I "+name, message, ConsoleColor.White);
        }

        public static void Success(string name, string message)
        {
            if (_config.Info.Successs)
                Texte("S " + name, message, ConsoleColor.Green);
        }

        public static void Notice(string name, string message)
        {
            if (_config.Info.Notice)
                Texte("N " + name, message, ConsoleColor.Yellow);
        }

        public static void Error(string name, string message)
        {
            if (_config.Info.Error)
                Texte("E " + name, message, ConsoleColor.Red);
        }

        public static void Debug(string name, string message)
        {
            if (_config.Info.Debug)
                Texte("D " + name, message, ConsoleColor.Blue);
        }

        public static void Dump(string name, string message)
        {
            if (_config.Info.Dump)
                Texte("D " + name, message, ConsoleColor.Gray);
        }

        public static bool CanDump()
        {
            return _config.Info.Dump;
        }

        public static void Tcp(string name, MemoryStream Packet, bool Force = false)
        {
            if (Force || _config.Info.Tcp)
            {
                byte[] Buff = Packet.ToArray();
                Texte("P " + name, Hex(Buff, 0, Buff.Length), ConsoleColor.Gray);
            }
        }

        public static void Tcp(string name, byte[] dump, int start, int len, bool Force=false)
        {
            if (Force || _config.Info.Tcp)
                Texte("P " + name, Hex(dump, start, len), ConsoleColor.Gray);
        }

        public static void Dump(string name, MemoryStream Packet, bool Force = false)
        {
            if (Force || _config.Info.Dump)
            {
                byte[] Buff = Packet.ToArray();
                Texte("U " + name, Hex(Buff, 0, Buff.Length), ConsoleColor.Gray);
            }
        }

        public static void Dump(string name, byte[] dump, int start, int len, bool Force = false)
        {
            if (_config.Info.Dump)
                Texte("U " + name, Hex(dump,start,len), ConsoleColor.Gray);
        }

        public static void Compare(string Name, byte[] First, byte[] Second)
        {
            if (_config.Info.Dump)
                return;

            if (First.Length != Second.Length)
                Log.Error("Name", "First.Length(" + First.Length + ") != Second.Length(" + Second.Length + ")");

            StringBuilder hex = new StringBuilder();

            for (int i = 0; i < Math.Max(First.Length, Second.Length); i += 16)
            {
                hex.Append("\n");

                bool LastDiff = false;
                for (int j = 0; j < 16; ++j)
                {
                    if (j + i < First.Length)
                    {
                        if (j + i < Second.Length)
                        {
                            if (First[j + i] != Second[j + i] && !LastDiff)
                            {
                                LastDiff = true;
                                hex.Append("[");
                            }
                            else if (First[j + i] == Second[j + i] && LastDiff)
                            {
                                LastDiff = false;
                                hex.Append("]");
                            }
                        }
                        else if (LastDiff)
                        {
                            LastDiff = false;
                            hex.Append("]");
                        }
                          

                        byte val = First[j + i];
                        //hex.Append(" ");
                        hex.Append(First[j + i].ToString("X2"));
                        if (j == 3 || j == 7 || j == 11)
                            hex.Append("");
                    }
                    else
                    {
                        hex.Append("  ");
                    }
                }
                if (LastDiff)
                {
                    LastDiff = false;
                    hex.Append("]");
                }

                hex.Append(" || ");

                LastDiff = false;
                for (int j = 0; j < 16; ++j)
                {
                    if (j + i < Second.Length)
                    {
                        if (j + i < First.Length)
                        {
                            if (First[j + i] != Second[j + i] && !LastDiff)
                            {
                                LastDiff = true;
                                hex.Append("[");
                            }
                            else if (First[j + i] == Second[j + i] && LastDiff)
                            {
                                LastDiff = false;
                                hex.Append("]");
                            }
                        }
                        else if (LastDiff)
                        {
                            LastDiff = false;
                            hex.Append("]");
                        }

                        byte val = Second[j + i];
                        //hex.Append(" ");
                        hex.Append(Second[j + i].ToString("X2"));
                        if (j == 3 || j == 7 || j == 11)
                            hex.Append("");
                    }
                    else
                    {
                        if (LastDiff)
                        {
                            LastDiff = false;
                            hex.Append("]");
                        }
  
                        hex.Append("  ");
                    }
                }
            }

            Texte("C " + Name, hex.ToString(), ConsoleColor.Gray);
        }

        public static string Hex(byte[] dump, int start, int len)
        {
            var hexDump = new StringBuilder();

            try
            {
                int end = start + len;
                for (int i = start; i < end; i += 16)
                {
                    StringBuilder text = new StringBuilder();
                    StringBuilder hex = new StringBuilder();
                    hex.Append("\n");

                    for (int j = 0; j < 16; j++)
                    {
                        if (j + i < end)
                        {
                            byte val = dump[j + i];
                            hex.Append(" ");
                            hex.Append(dump[j + i].ToString("X2"));
                            if (j == 3 || j == 7 || j == 11)
                                hex.Append(" ");
                            if (val >= 32 && val <= 127)
                            {
                                text.Append((char)val);
                            }
                            else
                            {
                                text.Append(".");
                            }
                        }
                        else
                        {
                            hex.Append("   ");
                            text.Append("  ");
                        }
                    }
                    hex.Append("  ");
                    hex.Append("//"+text.ToString());
                    hexDump.Append(hex.ToString());
                }
            }
            catch (Exception e)
            {
                Log.Error("HexDump", e.ToString());
            }

            return hexDump.ToString();
        }
    }
}
