/*
 * Copyright (C) 2013 APS
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
    /*public class AreaMapInfo
    {
        public UInt16 ZoneID;
        public List<Zone_Area> Areas;

        public AreaMapInfo(ushort ZoneID, List<Zone_Area> Areas)
        {
            this.ZoneID = ZoneID;
            this.Areas = Areas;
        }

        public Zone_Area GetArea(ushort PinX, ushort PinY, byte Realm, List<Zone_Area> Excepts=null)
        {
            foreach(Zone_Area Area in Areas)
            {
                if (Area.Realm == Realm && Area.IsOnArea(PinX,PinY))
                    if (Excepts == null || !Excepts.Contains(Area))
                        return Area;
            }

            return null;
        }

        public uint GetTokExplore(TokInterface Interface,ushort PinX, ushort PinY, byte Realm)
        {
            Zone_Area Area = GetArea(PinX, PinY, Realm);

            if (Area == null)
                return 0;

            if (Area.TokExploreEntry == 0)
                return 0;

            if (Interface != null && Interface.HasTok(Area.TokExploreEntry))
                return Area.TokExploreEntry;

            if (IsOnExploreArea(Area, PinX, PinY))
            {
                if (Interface != null)
                    Interface.AddTok(Area.TokExploreEntry);

                return Area.TokExploreEntry;
            }
            else
                return 0;
        }

        public uint GetInfluenceId(ushort PinX, ushort PinY, byte Realm)
        {
            Zone_Area Area = GetArea(PinX, PinY, Realm);

            if (Area == null)
                return 0;

            if (Area.InfluenceId == 0)
                return 0;

            if (IsOnExploreArea(Area, PinX, PinY))
                return Area.InfluenceId;
            else
                return 0;
        }

        public bool IsOnExploreArea(Zone_Area Area, ushort PinX, ushort PinY)
        {
            if (Area == null || Area.Information == null)
                return false;

            if (!Area.IsOnArea(PinX, PinY))
                return false;

            CheckArea(Area);

            if(Area.Information.File == null)
                return false;

            PinX = (ushort)(PinX / 64);
            PinY = (ushort)(PinY / 64);
            PinX -= Area.Information.OffsetX;
            PinY -= Area.Information.OffsetY;

            if (PinX >= Area.Information.Width || PinY >= Area.Information.Height || PinX < 0 || PinY < 0)
            {
                //Log.Error("IsOnExplore", "PinX=" + PinX + ",PinY=" + PinY+",ZoneId="+Area.ZoneId+",Piece="+Area.PieceId);
                return false;
            }

            System.Drawing.Color Col = Area.Information.File.GetPixel(PinX, PinY);
            if (Col.R == 255 && Col.G == 255 && Col.B == 255)
                return false;

            return true;
        }

        public void CheckArea(Zone_Area Area)
        {
            if (!Area.IsLoaded())
            {
                Area.Information.Loaded = true;
                string FileName = Program.Config.ZoneFolder + "zone" + String.Format("{0:000}", ZoneID) + "/piece" + String.Format("{0:00}", Area.Information.PieceId) + ".jpg";
                
                try
                {
                    Area.Information.File = new Bitmap(FileName);
                }
                catch (Exception e)
                {
                    Log.Error("AreaMapMgr", "Invalid Piece File : " + FileName + "\n" + e.ToString());
                }
            }
        }
    }

    static public class AreaMapMgr
    {
        static public Dictionary<int, AreaMapInfo> AreaInfos = new Dictionary<int, AreaMapInfo>();

        static public AreaMapInfo GetAreaInfo(ushort ZoneID)
        {
            AreaMapInfo Info;
            if (!AreaInfos.TryGetValue(ZoneID, out Info))
            {
                Info = new AreaMapInfo(ZoneID, WorldMgr.GetZoneAreas(ZoneID));
                AreaInfos.Add(ZoneID, Info);
            }

            return Info;
        }
    }*/
}
