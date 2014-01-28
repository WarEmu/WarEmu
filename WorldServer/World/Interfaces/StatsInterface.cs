
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class StatsInterface : BaseInterface
    {
        public UInt16[] _BaseStats = new UInt16[(int)GameData.Stats.STATS_COUNT];
        public UInt16[] _BonusStats = new UInt16[(int)GameData.Stats.STATS_COUNT];

        public StatsInterface(Object Obj)
            : base(Obj)
        {

        }

        public void Load(Dictionary<byte, UInt16> Stats)
        {
            if (IsLoad)
                return;

            foreach (KeyValuePair<byte, UInt16> Values in Stats)
                if (Values.Key < _BaseStats.Length && Values.Key >= 0)
                    _BaseStats[Values.Key] = Values.Value;

            base.Load();
        }
        public void Load(CharacterInfo_stats[] Stats)
        {
            if (IsLoad)
                return;

            foreach (CharacterInfo_stats Stat in Stats)
                if (Stat.StatId < _BaseStats.Length && Stat.StatId >= 0)
                    _BaseStats[Stat.StatId] = Stat.StatValue;

            base.Load();
        }
        public override void Update(long Tick)
        {

        }

        public UInt16[] GetStats()
        {
            return (UInt16[])_BaseStats.Clone();
        }
        public UInt16 GetBaseStat(byte Type)
        {
            return (ushort)_BaseStats[Type];
        }
        public UInt16 GetTotalStat(byte Type)
        {
            return (ushort)(_BaseStats[Type]+_BonusStats[Type]);
        }

        public void BuildStats(ref PacketOut Out)
        {
            Out.WriteByte(0);
            for (byte i = 0; i < _BaseStats.Length; ++i)
            {
                Out.WriteByte(i);
                Out.WriteUInt16(_BaseStats[i]);
            }
        }

        public void SetBaseStat(byte Type, UInt16 Stat)
        {
            if (Type >= _BaseStats.Length)
                return;

            _BaseStats[Type] = Stat;
        }
        public void AddBonusStat(byte Type, UInt16 Stat)
        {
            if(Type < _BonusStats.Length)
                _BonusStats[Type] += Stat;
        }
        public void RemoveBonusStat(byte Type, UInt16 Stat)
        {
            if (Type < _BonusStats.Length)
            if (_BonusStats[Type] >= Stat)
                _BonusStats[Type] -= Stat;
            else
                _BonusStats[Type] = 0;
        }
        public void ApplyStats()
        {
            if (Obj.IsUnit())
            {
                Unit Unit = Obj.GetUnit();
                Unit.MaxHealth = (uint)(GetTotalStat((byte)GameData.Stats.STATS_WOUNDS) * 10);
                if (Unit.Health > Unit.MaxHealth)
                    Unit.Health = Unit.MaxHealth;
            }

            if(Obj._Loaded && Obj.IsPlayer())
                Obj.GetPlayer().SendStats();
        }

        #region Combat

        public int CalculDamage()
        {
            if (!HasUnit())
                return 0;

            Unit Me = GetUnit();

            if (Me.IsPlayer())
            {
                float Str = GetTotalStat((byte)GameData.Stats.STATS_STRENGTH);
                float Tou = GetTotalStat((byte)GameData.Stats.STATS_TOUGHNESS);
                float Wdps = Me.ItmInterface.GetAttackDamage(EquipSlot.MAIN_DROITE) / 10;
                float WSpeed = Me.ItmInterface.GetAttackTime(EquipSlot.MAIN_DROITE);
                WSpeed /= 100;

                return (int)(((Str / 10) + Wdps) * WSpeed);
            }
            else if (Me.IsCreature())
            {
                int Dmg = 10 + 10 * Me.Level + (Me.Rank * 15);
                return Dmg;
            }

            return 1;
        }
        public int CalculReduce()
        {
            if (!HasUnit())
                return 0;

            Unit Me = GetUnit();
            int Reduce = 0;
            if (Me.IsPlayer())
            {
                UInt16 Tou = GetTotalStat((byte)GameData.Stats.STATS_TOUGHNESS);
                Reduce += Tou / 5;

                UInt16 Armor = Me.ItmInterface.GetEquipedArmor();
                Reduce += (int)(Armor / (Me.Level * 1.1));

                // TODO : Parry Skill et autre
            }
            else if (Me.IsCreature())
            {
                Reduce += 25;
            }

            return Reduce;
        }

        #endregion
    }
}
