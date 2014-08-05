/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
 * 
 * Copyright (C) 2011-2013 APS
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
using System.Drawing;
using System.IO;

using Common;
using FrameWork;

namespace WorldServer
{
    public struct AreaInfluence
    {
        public ushort AreaNumber;
        public byte Realm;
        public ushort InfluenceId;
    }

    public class MapPiece
    {
        public byte Id;
        public ushort ZoneId;
        public ushort PositionX, PositionY;
        public ushort SizeX, SizeY;
        public System.Drawing.Color[,] Colors;
        public Zone_Area Area;

        public bool IsPvp(byte Realm)
        {
            if (!Program.Config.OpenRvR && Area != null && Area.Realm != 0)
                return false;

            return true;
        }

        public bool IsOn(ushort PinX, ushort PinY, ushort ZoneId)
        {
            if (this.ZoneId != ZoneId)
                return false;

            if (PinX >= PositionX && PinX < PositionX + SizeX)
            {
                if (PinY >= PositionY && PinY < PositionY + SizeY)
                {
                    System.Drawing.Color Col = Colors[PinX - PositionX, PinY - PositionY];
                    if (Col.R != 255 && Col.G != 255 && Col.B != 255)
                        return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return "Id:" + Id + ",Area:" + Area;
        }
    }

    public class ClientZoneInfo
    {
        public ushort ZoneId;
        public string Folder;
        public List<AreaInfluence> Influences;
        public List<MapPiece> Pieces;
        public List<Zone_Area> Areas;
        public System.Drawing.Color[,] HeightMapOffset;
        public System.Drawing.Color[,] HeightMapTerrain;

        public ClientZoneInfo(ushort ZoneId)
        {
            this.ZoneId = ZoneId;
            this.Influences = new List<AreaInfluence>();
            this.Pieces = new List<MapPiece>();
            this.Folder = Program.Config.ZoneFolder + "zone" + String.Format("{0:000}", ZoneId) + "/";
            this.Areas = WorldMgr.GetZoneAreas(ZoneId);

            try
            {
                //LoadHeightMap();
                LoadInfluences();
                LoadMapPieces();

                FrameWork.Log.Success("ClientFile", ZoneId + " Loaded " + Influences.Count + " Influence(s), " + Pieces + " MapPiece(s)");
            }
            catch (Exception e)
            {
                FrameWork.Log.Error("ClientFile", e.ToString());
            }
        }

        public void LoadHeightMap()
        {
            int x, y;

            string FilePath = Path.Combine(Folder, "offset.png");
            if (File.Exists(FilePath))
            {
                using (Bitmap Map = new Bitmap(FilePath))
                {
                    HeightMapOffset = new System.Drawing.Color[Map.Width, Map.Height];
                    for (x = 0; x < Map.Width; ++x)
                    {
                        for (y = 0; y < Map.Height; ++y)
                        {
                            HeightMapOffset[x, y] = Map.GetPixel(x, y);
                        }
                    }
                }

                FilePath = Path.Combine(Folder, "terrain.png");
                using (Bitmap Map = new Bitmap(FilePath))
                {
                    HeightMapTerrain = new System.Drawing.Color[Map.Width, Map.Height];
                    for (x = 0; x < Map.Width; ++x)
                    {
                        for (y = 0; y < Map.Height; ++y)
                        {
                            HeightMapTerrain[x, y] = Map.GetPixel(x, y);
                        }
                    }
                }
            }
        }

        public void LoadMapPieces()
        {
            string FilePath = Path.Combine(Folder, "mappieces.csv");
            if (!File.Exists(FilePath))
                return;

            int x, y;
            using (FileStream Stream = File.OpenRead(FilePath))
            {
                using (StreamReader Reader = new StreamReader(Stream))
                {
                    Reader.ReadLine();
                    string Line = null;
                    while ((Line = Reader.ReadLine()) != null)
                    {
                        string[] Datas = Line.Split(',');
                        MapPiece Piece = new MapPiece();
                        Piece.ZoneId = ZoneId;
                        Piece.Id = byte.Parse(Datas[0]);
                        Piece.PositionX = ushort.Parse(Datas[1]);
                        Piece.PositionY = ushort.Parse(Datas[2]);
                        Piece.SizeX = ushort.Parse(Datas[3]);
                        Piece.SizeY = ushort.Parse(Datas[4]);
                        FilePath = Path.Combine(Folder, "piece" + String.Format("{0:00}", Piece.Id) + ".jpg");
                        using (Bitmap Map = new Bitmap(FilePath))
                        {
                            Piece.Colors = new System.Drawing.Color[Map.Width, Map.Height];
                            for (x = 0; x < Map.Width; ++x)
                            {
                                for (y = 0; y < Map.Height; ++y)
                                {
                                    Piece.Colors[x, y] = Map.GetPixel(x, y);
                                }
                            }

                        }
                        Pieces.Add(Piece);
                        Piece.Area = GetArea(Piece.Id);
                    }
                }
            }
        }

        public void LoadInfluences()
        {
            string FilePath = Path.Combine(Folder, "influenceids.csv");
            if (!File.Exists(FilePath))
                return;

            using (FileStream Stream = File.OpenRead(FilePath))
            {
                using (StreamReader Reader = new StreamReader(Stream))
                {
                    Reader.ReadLine();
                    string Line = null;
                    while ((Line = Reader.ReadLine()) != null)
                    {
                        string[] Datas = Line.Split(',');
                        AreaInfluence Area = new AreaInfluence();
                        Area.AreaNumber = ushort.Parse(Datas[0]);
                        Area.Realm = byte.Parse(Datas[1]);
                        Area.InfluenceId = ushort.Parse(Datas[2]);
                        Influences.Add(Area);
                    }
                }
            }
        }

        public Zone_Area GetArea(byte PieceId)
        {
            if(Areas != null)
                return Areas.Find(info => info.PieceId == PieceId);
            return null;
        }

        public MapPiece GetWorldPiece(ushort PinX, ushort PinY, ushort ZoneId)
        {
            PinX /= 64;
            PinY /= 64;
            foreach (MapPiece Piece in Pieces)
            {
                if (Piece.IsOn(PinX, PinY, ZoneId))
                {
                        return Piece;
                }
            }

            return null;
        }
    }

    public class HeighMapInfo
    {
        public HeighMapInfo(int ZoneID)
        {
            this.ZoneID = ZoneID;
        }

        public int ZoneID;
        public Bitmap Offset;
        public Bitmap Terrain;

        private bool Loaded = false;

        public int GetHeight(int PinX, int PinY)
        {
            Load();

            if (Offset == null || Terrain == null)
                return -1;

            PinX = (int)((float)PinX / 64f);
            PinY = (int)((float)PinY / 64f);

            if (PinX < 0 || PinX > Offset.Width || PinX > Terrain.Width)
                return -1;

            if (PinY < 0 || PinY > Offset.Height || PinY > Terrain.Height)
                return -1;

            float fZValue = 0;

            try
            {
                {
                    System.Drawing.Color iColor = Offset.GetPixel(PinX, PinY);
                    fZValue += iColor.R * 31; // 0 -> 30
                }

                {
                    System.Drawing.Color iColor = Terrain.GetPixel(PinX, PinY);
                    fZValue += iColor.R;
                }
            }
            catch (Exception e)
            {
                FrameWork.Log.Error("HeightMap", e.ToString());
            }

            fZValue *= 16;

            return (int)fZValue-30;
        }

        public void Load()
        {
            if (Loaded)
                return;

            Loaded = true;

            try
            {
                Offset = new Bitmap(Program.Config.ZoneFolder + "zone" + String.Format("{0:000}", ZoneID) + "/offset.png"); // /zones/zone003/offset.png
                Terrain = new Bitmap(Program.Config.ZoneFolder + "zone" + String.Format("{0:000}", ZoneID) + "/terrain.png"); // /zones/zone003/offset.png
            }
            catch(Exception e)
            {
                FrameWork.Log.Error("HeightMap", "[" + ZoneID + "] Invalid HeightMap \n " + e.ToString());
            }
        }
    }

    static public class ClientFileMgr
    {
        #region HeightMap Images

        static public Dictionary<int, HeighMapInfo> Heights = new Dictionary<int, HeighMapInfo>();

        static public int GetHeight(int ZoneID, int PinX, int PinY)
        {
            HeighMapInfo Info;
            if (!Heights.TryGetValue(ZoneID, out Info))
            {
                FrameWork.Log.Success("HeightMap", "["+ZoneID+"] Loading Height Map..");
                Info = new HeighMapInfo(ZoneID);
                Heights.Add(ZoneID, Info);
            }

            return Info.GetHeight(PinX, PinY) / 2;
        }

        #endregion

        #region MapPiece and CSV

        static public Dictionary<ushort, ClientZoneInfo> ClientZoneFiles = new Dictionary<ushort, ClientZoneInfo>();

        static public ClientZoneInfo GetZoneInfo(ushort ZoneId)
        {
            ClientZoneInfo Info;
            lock (ClientZoneFiles)
            {
                if (!ClientZoneFiles.TryGetValue(ZoneId, out Info))
                {
                    Info = new ClientZoneInfo(ZoneId);
                    ClientZoneFiles.Add(ZoneId, Info);
                }
            }
            return Info;
        }

        #endregion
    }
}
