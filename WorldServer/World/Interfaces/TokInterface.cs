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

using Common;
using FrameWork;

namespace WorldServer
{
    public class TokInterface : BaseInterface
    {
        public Dictionary<ushort, Character_tok> _Toks = new Dictionary<ushort, Character_tok>();

        public void Load(List<Character_tok> Toks)
        {
            if(Toks != null)
                foreach (Character_tok Tok in Toks)
                    _Toks.Add(Tok.TokEntry,Tok);

            base.Load();
        }
        public override void Save()
        {
            foreach (KeyValuePair<ushort, Character_tok> Kp in _Toks)
                CharMgr.Database.SaveObject(Kp.Value);
        }

        public bool HasTok(ushort Entry)
        {
            return _Toks.ContainsKey(Entry);
        }

        public void AddTok(Tok_Info Info)
        {
            if (Info != null)
                AddTok(Info.Entry);
        }
        public void AddTok(ushort Entry)
        {
            if (HasTok(Entry))
                return;

            Tok_Info Info = WorldMgr.GetTok(Entry);

            if (Info == null)
                return;

            SendTok(Entry, true);

            Character_tok Tok = new Character_tok();
            Tok.TokEntry = Entry;
            Tok.CharacterId = GetPlayer().CharacterId;
            Tok.Count = 1;

            _Toks.Add(Entry, Tok);
            GetPlayer().AddXp(Info.Xp);
            GetPlayer()._Info.Toks = _Toks.Values.ToList();

            CharMgr.Database.AddObject(Tok);
        }
        public void SendAllToks()
        {
            foreach (KeyValuePair<ushort, Character_tok> Kp in _Toks)
                SendTok(Kp.Value.TokEntry, false);

            if (Program.Config.DiscoverAll)
            {
                foreach (Tok_Info Info in WorldMgr.DiscoveringToks)
                    SendTok(Info.Entry, false);
            }
        }
        public void SendTok(ushort Entry, bool Print)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_TOK_ENTRY_UPDATE);
            Out.WriteUInt32(1);
            Out.WriteUInt16((UInt16)Entry);
            Out.WriteByte(1);
            Out.WriteByte((byte)(Print ? 1 : 0));
            Out.WriteByte(1);
            GetPlayer().SendPacket(Out);
        }
    }
}
