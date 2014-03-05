
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using Common;

namespace WorldServer
{
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
            FrameWork.Log.Success("pin x pin y ", " = " + PinX + "  " + PinY);


            if (this.ZoneID == 130)
            {
                PinX = PinX - 327684;
                PinY = PinY - 327684;
            }
            else if (this.ZoneID == 100)
            {
                PinX = PinX - 819220;
                PinY = PinY - 819220;
            }


            FrameWork.Log.Success("Worldmap pin x pin y - 327780", " = " + PinX + "  " + PinY);
            
            Load();

            if (Offset == null || Terrain == null)
                return -1;



            PinX = (int)((float)PinX / 64f);
            PinY = (int)((float)PinY / 64f);

            FrameWork.Log.Success("pin x pin y / 64 offset", " = " + PinX+"  "+PinY);

            if (PinX < 0 || PinX > Offset.Width || PinX > Terrain.Width)
                return -1;

            if (PinY < 0 || PinY > Offset.Height || PinY > Terrain.Height)
                return -1;

            float fZValue = 0;

            try
            {
                {
                    Color iColor = Offset.GetPixel(PinX, PinY);
                    FrameWork.Log.Success("icolor offset", " = " + iColor.R);
                    fZValue += iColor.R * 31; // 0 -> 30
                    FrameWork.Log.Success("fZValue", " = " + fZValue);
                    
                }

                {
                    Color iColor = Terrain.GetPixel(PinX, PinY);
                    FrameWork.Log.Success("icolor terrain", " = " + iColor.R);
                    fZValue += iColor.R;
                    FrameWork.Log.Success("fZValue", " = " + fZValue);
                    
                }
            }
            catch (Exception e)
            {
                FrameWork.Log.Error("HeightMap", e.ToString());
            }

            fZValue *= 8;  // was 16
            FrameWork.Log.Success("fZValue", " = " + fZValue);
            FrameWork.Log.Success("fZValue", " = " +(fZValue - 30));
            return (int)fZValue + 15;    // removed -30
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

    static public class HeightMapMgr
    {
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

            return Info.GetHeight(PinX, PinY);
        }
    }
}
