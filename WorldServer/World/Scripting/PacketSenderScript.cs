using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Common;
using FrameWork;

namespace WorldServer
{
    [GeneralScript(true, "CareerPackagesSender", 0, 0)]
    public class CareerPackagesSender : AGeneralScript
    {
        public bool Inited = false;
        public Dictionary<byte, List<PacketInfo>> Packets = new Dictionary<byte, List<PacketInfo>>();
        public Dictionary<byte, PacketInfo> Intros = new Dictionary<byte, PacketInfo>();

        public bool CanAnalyze(string OpcodeName)
        {
            if (OpcodeName == "F_CAREER_PACKAGE_INFO")
                return true;

            if (OpcodeName == "F_CAREER_CATEGORY")
                return true;

            if (OpcodeName == "S_PLAYER_INITTED")
                return true;

            if (OpcodeName == "F_INTRO_CINEMA")
                return true;

            return false;
        }

        public override void OnInitObject(Object Obj)
        {
            CheckInit();
        }

        public override void OnWorldPlayerEvent(string EventName, Player Plr, object Data)
        {
            if (EventName == "SEND_PACKAGES")
            {
                List<PacketInfo> P;
                if (Packets.TryGetValue(Plr._Info.Career, out P))
                {
                    foreach (PacketInfo Packet in P)
                    {
                        PacketOut Out = new PacketOut(Packet.Opcode);
                        Out.Write(Packet.Data, 3, Packet.Data.Length - 3);
                        Plr.SendPacket(Out);
                    }
                }

                if (Plr._Info.FirstConnect && Plr.GmLevel == 0)
                {
                    Plr._Info.FirstConnect = false;
                    PacketInfo Packet = Intros[Plr._Info.Career];
                    PacketOut Out = new PacketOut(Packet.Opcode);
                    Out.Write(Packet.Data, 3, Packet.Data.Length - 3);
                    Plr.SendPacket(Out);
                }
            }
        }

        public void CheckInit()
        {
            lock (Packets)
            {
                if (!Inited)
                {
                    int Count = 0;
                    string[] Files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Abilities/"), "*.txt", SearchOption.TopDirectoryOnly);
                    foreach (string P in Files)
                    {
                        SniffFile SFile = new SniffFile();
                        SFile.FilePath = P;
                        SFile.Analyze(CanAnalyze);

                        byte CurrentCareer = 0;
                        List<PacketInfo> L = null;
                        foreach (PacketInfo Info in SFile.Packets)
                        {
                            if (Info.Opcode == 0x88) // INITED
                            {
                                CurrentCareer = Info.Data[33 + 3];
                                if (!Packets.ContainsKey(CurrentCareer))
                                {
                                    L = new List<PacketInfo>();
                                    Packets.Add(CurrentCareer, L);
                                }
                                else
                                    L = Packets[CurrentCareer];
                            }
                            else if (CanAnalyze(Info.OpcodeName))
                            {
                                if (Info.OpcodeName != "F_INTRO_CINEMA")
                                {
                                    L.Add(Info);
                                    ++Count;
                                }
                                else
                                {
                                    Intros.Add(CurrentCareer, Info);
                                }
                            }
                        }
                    }

                    Log.Success("Packages", "Loaded " + Count + " packages");
                }
            }
        }
    }

    public class Analyzer
    {
        public string[] StrAnalyze = null;
        public PacketsContainer Container;

        public virtual void Init(PacketsContainer Container)
        {
            this.Container = Container;
        }

        public virtual string[] GetAnalyze()
        {
            return StrAnalyze;
        }

        public virtual void Analyze(object obj)
        {

        }
    }

    public class PacketsContainer
    {
        public SniffFile File;
        public byte Opcode;
        public string OpcodeName;
        public List<PacketInfo> Packets = new List<PacketInfo>();

        public bool Analyzed = false;
        public bool Imported = false;
        public bool Extracted = false;
        public bool ShowAnalyse = false;
        public Analyzer Analyzer;

        public string[] GetAnalyze()
        {
            if (Analyzer != null)
                return Analyzer.GetAnalyze();

            return null;
        }

        //public List<WorldObject> Objects = new List<WorldObject>();
    }

    public class PacketInfo
    {
        public PacketInfo(SniffFile Parent)
        {
            this.Parent = Parent;
        }
        public SniffFile Parent;
        public int Size;
        public byte Opcode;
        public string OpcodeName;
        public byte[] Data;
        public int Line = 0;
    }

    public class SniffFile
    {
        public delegate bool CanAnalyseDelegate(string OpcodeName);

        public string FilePath;
        public int TotalPackets = 0;
        public int Region = 0;
        public List<PacketInfo> Packets = new List<PacketInfo>();
        public Dictionary<byte, PacketsContainer> PacketsAnalyze = new Dictionary<byte, PacketsContainer>();

        public bool Analyzed = false;
        public bool Imported = false;
        public bool Extracted = false;
        public bool ShowAnalyse = false;
        public byte State = 0;
        // 0 = White
        // 1 = Grey
        // 2 = Green
        // 3 = Red

        public void AddPacket(PacketInfo Packet)
        {
            Packets.Add(Packet);
            PacketsContainer TempPackets = null;
            if (!PacketsAnalyze.TryGetValue(Packet.Opcode, out TempPackets))
            {
                TempPackets = new PacketsContainer();
                TempPackets.File = this;
                PacketsAnalyze.Add(Packet.Opcode, TempPackets);
                TempPackets.Opcode = Packet.Opcode;
                TempPackets.OpcodeName = Packet.OpcodeName;
            }

            TempPackets.Packets.Add(Packet);
        }

        public PacketsContainer GetPackets(byte Opcode)
        {
            PacketsContainer Container;
            PacketsAnalyze.TryGetValue(Opcode, out Container);
            return Container;
        }

        public PacketsContainer GetPackets(string Name)
        {
            foreach (KeyValuePair<byte, PacketsContainer> Kp in PacketsAnalyze)
                if (Kp.Value.OpcodeName == Name)
                    return Kp.Value;

            return null;
        }

        public Analyzer GetAnalyzer(string Name)
        {
            PacketsContainer Container = GetPackets(Name);
            if (Container != null)
                return Container.Analyzer;

            return null;
        }

        public void Analyze(CanAnalyseDelegate CanAnalyze)
        {
            using (StreamReader SReader = File.OpenText(FilePath))
            {
                string Line = "";
                int TempIndex = 0;
                int TempPosition = 0;
                byte TempOpcode = 0;
                string TempOpcodeName;
                PacketInfo Packet = null;
                int LineId = 0;
                int MaxPosition = 0;

                while (!SReader.EndOfStream)
                {
                    Line = SReader.ReadLine();
                    if (Line.Length == 0)
                        continue;

                    if (Line[0] == '[')
                    {
                        ++TotalPackets;
                        TempIndex = Line.IndexOf('(');
                        TempOpcodeName = Line.Substring(TempIndex + 7, Line.IndexOf("  ") - (TempIndex + 7));

                        if (!CanAnalyze(TempOpcodeName))
                        {
                            if (!SReader.EndOfStream) Line = SReader.ReadLine();
                            if (!SReader.EndOfStream) Line = SReader.ReadLine();
                            if (!SReader.EndOfStream) Line = SReader.ReadLine();
                            while (!SReader.EndOfStream)
                            {
                                Line = SReader.ReadLine();

                                if (Line[0] == '-')
                                    break;
                            }
                            continue;
                        }

                        TempOpcode = byte.Parse(Line.Substring(TempIndex + 3, 2), System.Globalization.NumberStyles.HexNumber);

                        Packet = new PacketInfo(this);

                        TempIndex = Line.LastIndexOf('=') + 1;
                        Packet.Size = int.Parse(Line.Substring(TempIndex, Line.Length - TempIndex));
                        Packet.Opcode = TempOpcode;
                        Packet.OpcodeName = TempOpcodeName;
                        Packet.Data = new byte[Packet.Size];
                        if (!SReader.EndOfStream)
                            Line = SReader.ReadLine(); // |------------------------------------------------|----------------|
                        if (!SReader.EndOfStream)
                            Line = SReader.ReadLine(); // |00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F |0123456789ABCDEF|
                        if (!SReader.EndOfStream)
                            Line = SReader.ReadLine(); // |------------------------------------------------|----------------|

                        Packet.Line = LineId;
                        TempPosition = 0;

                        while (!SReader.EndOfStream)
                        {
                            Line = SReader.ReadLine();

                            if (Line[0] == '-')
                                break;

                            Line = Line.Replace(" ", string.Empty);

                            MaxPosition = Math.Min(33, (Packet.Size - TempPosition) * 2);
                            for (TempIndex = 1; TempIndex < MaxPosition; TempIndex += 2)
                            {
                                Packet.Data[TempPosition] = byte.Parse(Line.Substring(TempIndex, 2), System.Globalization.NumberStyles.HexNumber);
                                ++TempPosition;
                            }
                        }

                        ++LineId;

                        AddPacket(Packet);
                    }
                }
            }
        }
    }
}