
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FrameWork
{
    static public class Utils
    {
        static public List<T> ConvertStringToArray<T>(string Value)
        {
            string[] Values = Value.Split(' ');
            List<T> L = new List<T>();
            foreach (string Val in Values)
                if (Val.Length > 0)
                {
                    try
                    {
                        L.Add((T)Convert.ChangeType(Val, typeof(T)));
                    }
                    catch
                    {
                        L.Add((T)Convert.ChangeType("0",typeof(T)));
                    }
                }

            return L;
        }
        static public string ConvertArrayToString<T>(T[] Value)
        {
            string Result = "";
            foreach (T val in Value)
                Result += (string)Convert.ChangeType(val, typeof(string)) + " ";

            return Result;
        }

        static public string VectorToString(Vector3 Vec)
        {
            return PositionToString(Vec.X, Vec.Y, Vec.Z);
        }
        static public Vector3 StringToVector(string Str)
        {
            float[] Value = StringToPosition(Str);
            return new Vector3(Value[0], Value[1], Value[2]);
        }

        static public string PositionToString(float X, float Y, float Z)
        {
            return "" + X + ":" + Y + ":" + Z;
        }
        static public float[] StringToPosition(string Str)
        {
            float[] Result = new float[3] { 0, 0, 0 };
            string[] Value = Str.Split(':');
            for (int i = 0; i < Result.Length; ++i)
                if (Value.Length >= i)
                    Result[i] = float.Parse(Value[i]);
            return Result;
        }

        static public string ChannelsToString(Dictionary<int, Color> Chans)
        {
            string Result = "";

            if (Chans.Count <= 0)
                return Result;

            foreach (KeyValuePair<int, Color> Kp in Chans)
                Result += Kp.Key.ToString() + "," + ColorToString(Kp.Value) + "|";

            // 0 - 1 - 2    
            return Result.Remove(Result.Length - 1, 1);
        }
        static public Dictionary<int, Color> StringToChannels(string Str)
        {
            // 1,18:20:25:45|2,457:154:651:0
            Dictionary<int, Color> Chans = new Dictionary<int, Color>();
            if (Str.Length <= 0)
                return Chans;

            foreach (string Kp in Str.Split('|'))
            {
                if (Kp.Length <= 0)
                    continue;

                int ChannelId = int.Parse(Kp.Split(',')[0]);
                Color Col = StringToColor(Kp.Split(',')[1]);

                if (!Chans.ContainsKey(ChannelId))
                    Chans.Add(ChannelId, Col);
            }
            return Chans;
        }

        static public string IntArrayToString(int[] Values)
        {
            string Result = "";

            if (Values.Length <= 0)
                return Result;

            foreach (int Val in Values)
                Result += Val.ToString() + ":";

            return Result.Remove(Result.Length - 1, 1);
        }
        static public int[] StringToIntArray(string Str)
        {
            if (Str.Length <= 0)
                return new int[0];

            string[] Values = Str.Split(':');
            int[] Result = new int[Values.Length];

            for (int i = 0; i < Values.Length; ++i)
                if (Values[i].Length > 0)
                    Result[i] = int.Parse(Values[i]);

            return Result;
        }

        static public string EquipementToString(Dictionary<int, int> Chans)
        {
            string Result = "";

            if (Chans.Count <= 0)
                return Result;

            foreach (KeyValuePair<int, int> Kp in Chans)
                Result += Kp.Key.ToString() + "," + Kp.Value.ToString() + "|";

            return Result.Remove(Result.Length - 1, 1);
        }
        static public Dictionary<int, int> StringToEquipement(string Str)
        {
            // 1,134|2,18
            Dictionary<int, int> Chans = new Dictionary<int, int>();
            if (Str.Length <= 0)
                return Chans;

            foreach (string Kp in Str.Split('|'))
            {
                if (Kp.Length <= 0)
                    continue;

                int ChannelId = int.Parse(Kp.Split(',')[0]);
                int ItemId = int.Parse(Kp.Split(',')[1]);

                if (!Chans.ContainsKey(ChannelId))
                    Chans.Add(ChannelId, ItemId);
            }
            return Chans;
        }

        static public string ColorToString(Color Col)
        {
            return Col.R + ":" + Col.G + ":" + Col.B + ":" + Col.A;
        }
        static public Color StringToColor(string Str)
        {
            string[] Bytes = Str.Split(':');
            Color Col = new Color();
            Col.R = float.Parse(Bytes[0]);
            Col.G = float.Parse(Bytes[1]);
            Col.B = float.Parse(Bytes[2]);
            Col.A = float.Parse(Bytes[3]);
            return Col;

        }
        static public String AssemblyVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        static public long Encode2Values(int P1, int P2)
        {
            return (long)(P1 << 32 + P2);
        }
        static public void Decode2Values(long Value, out int P1, out int P2)
        {
            P1 = (int)(Value >> 32);
            P2 = (int)(Value & 0xFFFFFFFF);
        }

        static public int Encode2Values(ushort P1, ushort P2)
        {
            return (int)(P1 << 16 + P2);
        }
        static public void Decode2Values(int Value, out ushort P1, out ushort P2)
        {
            P1 = (ushort)(Value >> 16);
            P2 = (ushort)(Value & 0xFFFF);
        }

        static public ushort Encode2Values(byte P1, byte P2)
        {
            return (ushort)((P1 << 8) + P2);
        }
        static public void Decode2Values(ushort Value, out byte P1, out byte P2)
        {
            P1 = (byte)(Value >> 8);
            P2 = (byte)(Value & 0xFF);
        }

        static public int Encode4Values(byte P1, byte P2, byte P3, byte P4)
        {
            return (int)((P1 << 24) + (P2 << 16) + (P3 << 8) + P4);
        }
        static public void Decode4Values(int Value, out byte P1, out byte P2, out byte P3, out byte P4)
        {
            P1 = (byte)(Value >> 24);
            P2 = (byte)(Value >> 16);
            P3 = (byte)(Value >> 8);
            P4 = (byte)(Value & 0xFF);
        }
    }
}
