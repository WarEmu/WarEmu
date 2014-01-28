
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;
namespace WorldServer
{
    public class PQuestObject : Object
    {
        public PQuest_Info Info;

        public PQuestObject()
            : base()
        {

        }

        public PQuestObject(PQuest_Info Info)
            : this()
        {
            this.Info = Info;
            Name = Info.Name;
        }

        public override void OnLoad()
        {
            X = (int)Info.PinX;
            Y = (int)Info.PinY;
            Z = 16384;
            SetOffset(Info.OffX, Info.OffY);
            Region.UpdateRange(this);

            base.OnLoad();
        }

        public override void SendMeTo(Player Plr)
        {
            // TODO
            // Send Quest Info && Current Stage && Current Players
        }
    }
}
