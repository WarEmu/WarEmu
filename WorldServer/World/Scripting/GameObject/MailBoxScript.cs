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

using Common;
using FrameWork;

namespace WorldServer
{
    [GeneralScript(true,"MailBoxScript", 0 , 0)]
    public class MailBoxScript : AGeneralScript
    {
        public override void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {
            Log.Info("MailBox", "OnInteract " + Target.ToString());

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
                Out.WriteUInt16(3);
                Out.WriteUInt16(0);
                Out.WriteByte(1);
                Target.SendPacket(Out);
            }


            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
                Out.WriteUInt16(10);
                Out.WriteUInt32(0x0E000000);
                Out.WriteUInt32(0x001E0AD7);
                Out.WriteUInt16(0xA33C);
                Target.SendPacket(Out);
            }


            {
                //MailsMgr.SendMails(Target);
            }
        }
    }
}
