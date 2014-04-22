
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;
namespace WorldServer
{
    public class ChapterObject : Object
    {
        public Chapter_Info Info;

        public ChapterObject()
            : base()
        {

        }

        public ChapterObject(Chapter_Info Info)
            : this()
        {
            this.Info = Info;
            Name = Info.Name;
        }

        public override void OnLoad()
        {
            X = Info.PinX;
            Y = Info.PinY;
            Z = 16384;
            SetOffset(Info.OffX, Info.OffY);
            IsActive = true;

            base.OnLoad();
        }

        public override void SendMeTo(Player Plr)
        {
            Plr.TokInterface.AddTok(Info.TokEntry);
        }
    }
}
