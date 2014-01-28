
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using nsHashDictionary;
using MYPHandler;

namespace Launcher
{
    static public class Client
    {
        static public int Version = 1;
        static public string IP = "127.0.0.1";
        static public int Port = 8000;
        static public bool Started = false;

        static public string User;
        static public string Auth;
        static public string Language = "";

        // TCP
        static public Socket _Socket;

        static public void Print(string Message)
        {
            Accueil.Acc.Print(Message);
        }

        static public bool Connect()
        {
            try
            {
                if (_Socket != null)
                    _Socket.Close();

                _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _Socket.Connect(IP, Port);

                BeginReceive();

                SendCheck();
            }
            catch (Exception e)
            {
                MessageBox.Show("Can not connect to : " + IP + ":" + Port + "\n" + e.Message);
                return false;
            }

            return true;
        }

        static public void Close()
        {
            try
            {
                if (_Socket != null)
                    _Socket.Close();
              
            }
            catch(Exception)
            {

            }
        }

        static public void UpdateLanguage()
        {
            if (Language.Length <= 0)
                return;

            int LangueId = 1;
            switch (Language)
            {
                case "French":
                    LangueId = 2;
                    break;
                case "English":
                    LangueId = 1;
                    break;
                case "Deutch":
                    LangueId = 3;
                    break;
                case "Italian":
                    LangueId = 4; 
                    break;
                case "Spanish":
                    LangueId = 5;
                    break;
                case "Korean":
                    LangueId = 6;
                    break;
                case "Chinese":
                    LangueId = 7;
                    break;
                case "Japanese":
                    LangueId = 9;
                    break;
                case "Russian":
                    LangueId = 10;
                    break;
            };

            string CurDir = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(CurDir + "\\..\\user\\");

                StreamReader Reader = new StreamReader("UserSettings.xml");
                string line = "";
                string TotalStream = "";
                while ( (line = Reader.ReadLine()) != null)
                {
                    Print(line);
                    int Pos = line.IndexOf("Language id=");
                    if (Pos > 0)
                    {
                        Pos = line.IndexOf("\"")+1;
                        int Pos2 = line.LastIndexOf("\"");
                        line = line.Remove(Pos, Pos2-Pos);
                        line = line.Insert(Pos, "" + LangueId);
                    }

                    TotalStream += line +"\n";
                }
                Reader.Close();

                StreamWriter Writer = new StreamWriter("UserSettings.xml", false);
                Writer.Write(TotalStream);
                Writer.Flush();
                Writer.Close();
            }
            catch (Exception e)
            {
                Print("Ecriture : " + e.ToString());
            }
        }

        static public void UpdateRealms()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.CL_INFO);
            SendTCP(Out);
        }

        #region Sender

        // Buffer en train d'être envoyé
        static byte[] m_tcpSendBuffer = new byte[65000];

        // Liste des packets a sender
        static readonly Queue<byte[]> m_tcpQueue = new Queue<byte[]>(256);

        // True si un send est en cours
        static bool m_sendingTcp = false;

        // Envoi un packet
        static public void SendTCP(PacketOut packet)
        {
            //Fix the packet size
            packet.WritePacketLength();

            //Get the packet buffer
            byte[] buf = packet.GetBuffer(); //packet.WritePacketLength sets the Capacity

            //Send the buffer
            SendTCP(buf);
        }

        static public void SendTCP(byte[] buf)
        {
            if (m_tcpSendBuffer == null)
                return;

            //Check if client is connected
            if (_Socket.Connected)
            {

                try
                {
                    lock (m_tcpQueue)
                    {
                        if (m_sendingTcp)
                        {
                            m_tcpQueue.Enqueue(buf);
                            return;
                        }

                        m_sendingTcp = true;
                    }

                    Buffer.BlockCopy(buf, 0, m_tcpSendBuffer, 0, buf.Length);

                    _Socket.BeginSend(m_tcpSendBuffer, 0, buf.Length, SocketFlags.None, m_asyncTcpCallback, null);
                }
                catch
                {
                    Close();
                }
            }
        }

        static readonly AsyncCallback m_asyncTcpCallback = AsyncTcpSendCallback;

        static void AsyncTcpSendCallback(IAsyncResult ar)
        {
            try
            {
                Queue<byte[]> q = m_tcpQueue;

                int sent = _Socket.EndSend(ar);

                int count = 0;
                byte[] data = m_tcpSendBuffer;

                if (data == null)
                    return;

                lock (q)
                {
                    if (q.Count > 0)
                    {
                        //						Log.WarnFormat("async sent {0} bytes, sending queued packets count: {1}", sent, q.Count);
                        count = CombinePackets(data, q, data.Length);
                    }
                    if (count <= 0)
                    {
                        //						Log.WarnFormat("async sent {0} bytes", sent);
                        m_sendingTcp = false;
                        return;
                    }
                }

                _Socket.BeginSend(data, 0, count, SocketFlags.None, m_asyncTcpCallback, null);

            }
            catch (Exception)
            {
                    Close();
            }
        }

        private static int CombinePackets(byte[] buf, Queue<byte[]> q, int length)
        {
            int i = 0;
            do
            {
                var pak = q.Peek();
                if (i + pak.Length > buf.Length)
                {
                    if (i == 0)
                    {
                        q.Dequeue();
                        continue;
                    }
                    break;
                }

                Buffer.BlockCopy(pak, 0, buf, i, pak.Length);
                i += pak.Length;

                q.Dequeue();
            } while (q.Count > 0);

            return i;
        }

        static public void SendTCPRaw(PacketOut packet)
        {
            SendTCP((byte[])packet.GetBuffer().Clone());
        }

        #endregion

        #region Receiver

        static private readonly AsyncCallback ReceiveCallback = OnReceiveHandler;
        static byte[] _pBuf = new byte[2048];


        private static void OnReceiveHandler(IAsyncResult ar)
        {
            try
            {
                int numBytes = _Socket.EndReceive(ar);

                if (numBytes > 0)
                {
                    byte[] buffer = _pBuf;
                    int bufferSize = numBytes;

                    PacketIn pack = new PacketIn(buffer, 0, bufferSize);
                    OnReceive(pack);
                    BeginReceive();

                }
                else
                {
                    Close();
                }
            }
            catch
            {

            }
        }

        static public void BeginReceive()
        {
            if (_Socket != null && _Socket.Connected)
            {
                int bufSize = _pBuf.Length;

                if (0 >= bufSize) //Do we have space to receive?
                {
                    Close();
                }
                else
                {
                    _Socket.BeginReceive(_pBuf, 0, bufSize, SocketFlags.None, ReceiveCallback, null);
                }
            }
        }

        #endregion

        static public void OnReceive(PacketIn packet)
        {
            lock (packet)
            {
                packet.Size = packet.GetUint32();
                packet.Opcode = packet.GetUint8();

                Handle(packet);
            }
        }

        #region Packet

        static public void Handle(PacketIn packet)
        {
            if(!Enum.IsDefined(typeof(Opcodes),(byte)packet.Opcode))
            {
                Print("Invalid opcode : " + packet.Opcode.ToString("X02"));
                return;
            }

            switch((Opcodes)packet.Opcode)
            {
                case Opcodes.LCR_CHECK:
                    
                    byte Result = packet.GetUint8();

                    switch((CheckResult)Result)
                    {
                        case CheckResult.LAUNCHER_OK:
                            Start();
                            break;
                        case CheckResult.LAUNCHER_VERSION:
                            string Message = packet.GetString();
                            Print(Message);
                            Close();
                            break;
                        case CheckResult.LAUNCHER_FILE:

                            string File = packet.GetString();
                            byte[] Bt = ASCIIEncoding.ASCII.GetBytes(File);

                            FileInfo Info = new FileInfo("mythloginserviceconfig.xml");
                            FileStream Str = Info.Create();
                            Str.Write(Bt, 0, Bt.Length);
                            Str.Close();
                            break;
                    }
                    break;

                case Opcodes.LCR_START:

                    Accueil.Acc.ReceiveStart();

                    byte Res = packet.GetUint8();

                    if (Res >= 1)
                    {
                        Print("Error : wrong account !\n\r");
                        return;
                    }
                    else
                    {
                        Auth = packet.GetString();
                        Print("Lancement avec : " + Auth);
                        try
                        {
                            string CurrentDir = Directory.GetCurrentDirectory();
                            patchExe();
                            UpdateWarData();
                            Process Pro = new Process();
                            Pro.StartInfo.FileName = "WAR.exe";
                            Pro.StartInfo.Arguments = " --acctname=" + System.Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(User)) + " --sesstoken=" + System.Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Auth));
                            Pro.Start();
                            Directory.SetCurrentDirectory(CurrentDir);
                        }
                        catch (Exception e)
                        {
                            Print(e.ToString());
                        }
                    }

                    break;

                case Opcodes.LCR_INFO:
                    {
                        Accueil.Acc.ClearRealms();
                        byte RealmsCount = packet.GetUint8();
                        for (byte i = 0; i < RealmsCount; ++i)
                        {
                            bool Online = packet.GetUint8() > 0;
                            string Name = packet.GetString();
                            UInt32 OnlinePlayers = packet.GetUint32();
                            UInt32 OrderCount = packet.GetUint32();
                            UInt32 DestructionCount = packet.GetUint32();

                            Accueil.Acc.AddRealm(Name, Online, OnlinePlayers, DestructionCount, DestructionCount);

                        }
                    }break;
            }
        }

        static public void Start()
        {
            if(Started)
                return;

           Started = true;
        }

        static public void SendCheck()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.CL_CHECK);
            Out.WriteUInt32((uint)Version);

            FileInfo Info = new FileInfo("mythloginserviceconfig.xml");
            if (Info.Exists)
            {
                Out.WriteByte(1);
                Out.WriteUInt64((ulong)Info.Length);
            }
            else
            {
                Out.WriteByte(0);
            }

            SendTCP(Out);
        }
        public static void patchExe()
        {
            using (Stream stream = new FileStream(Directory.GetCurrentDirectory() + "\\..\\WAR.exe", FileMode.OpenOrCreate))
            {

                int encryptAddress = (0x00957FBE + 3) - 0x00400000;
                stream.Seek(encryptAddress, SeekOrigin.Begin);
                stream.WriteByte(0x01);



                byte[] decryptPatch1 = { 0x90, 0x90, 0x90, 0x90, 0x57, 0x8B, 0xF8, 0xEB, 0x32 };
                int decryptAddress1 = (0x009580CB) - 0x00400000;
                stream.Seek(decryptAddress1, SeekOrigin.Begin);
                stream.Write(decryptPatch1, 0, 9);

                byte[] decryptPatch2 = { 0x90, 0x90, 0x90, 0x90, 0xEB, 0x08 };
                int decryptAddress2 = (0x0095814B) - 0x00400000;
                stream.Seek(decryptAddress2, SeekOrigin.Begin);
                stream.Write(decryptPatch2, 0, 6);

                //stream.WriteByte(0x01);
            }
        }
        static public void UpdateWarData()
        {
            try
            {
                FileStream fs = new FileStream(Application.StartupPath + "\\mythloginserviceconfig.xml", FileMode.Open, FileAccess.Read);

                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\..\\");

                HashDictionary hashDictionary = new HashDictionary();
                hashDictionary.AddHash(0x3FE03665, 0x349E2A8C, "mythloginserviceconfig.xml", 0);
                MYPHandler.MYPHandler mypHandler = new MYPHandler.MYPHandler("data.myp", null, null, hashDictionary);
                mypHandler.GetFileTable();

                FileInArchive theFile = mypHandler.SearchForFile("mythloginserviceconfig.xml");

                if (theFile == null)
                {
                    MessageBox.Show("Can not find config file in data.myp");
                    return;
                }

                if (File.Exists(Application.StartupPath + "\\mythloginserviceconfig.xml") == false)
                {
                    MessageBox.Show("Missing file : mythloginserviceconfig.xml");
                    return;
                }

                mypHandler.ReplaceFile(theFile, fs);

                fs.Close();
            }
            catch (Exception e)
            {
                Print(e.ToString());
            }
        }





        #endregion
    }
}
