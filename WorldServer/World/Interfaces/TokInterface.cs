
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
        public Dictionary<UInt32, Character_tok> _Toks = new Dictionary<uint, Character_tok>();
        public TokInterface(Object Obj)
            : base(Obj)
        {

        }

        public void Load(Character_tok[] Toks)
        {
            if(Toks != null)
                foreach (Character_tok Tok in Toks)
                    _Toks.Add(Tok.TokEntry,Tok);

            base.Load();
        }
        public bool HasTok(UInt32 Entry)
        {
            return _Toks.ContainsKey(Entry);
        }
        public void AddTok(UInt32 Entry)
        {
            if (HasTok(Entry))
                return;

            Tok_Info Info = WorldMgr.GetTok(Entry);

            if (Info == null)
                return;

            SendTok(Entry,true);

            Character_tok Tok = new Character_tok();
            Tok.TokEntry = Entry;
            Tok.CharacterId = GetPlayer().CharacterId;
            Tok.Count = 1;
            Tok.Dirty = true;

            _Toks.Add(Entry, Tok);
            GetPlayer().AddXp(Info.Xp);
            GetPlayer()._Info.Toks = _Toks.Values.ToArray();

            CharMgr.Database.AddObject(Tok);
        }
        public void SendAllToks()
        {
            if (!HasPlayer())
                return;

            foreach (Character_tok Tok in _Toks.Values)
                SendTok(Tok.TokEntry, false);
        }
        public void SendTok(UInt32 Entry, bool Print)
        {
            if (!HasPlayer())
                return;

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
