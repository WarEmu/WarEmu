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
    [IAbilityType("DealDamages", "Deal Damage Handler")]
    public class DealDamagesHandler : IAbilityTypeHandler
    {
        public Ability Ab;
        public Unit Target;
        uint OffensiveStat = 0;
        uint DefensiveStat = 0;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;

            if (Ab.Caster.IsUnit())
            {
                Target = Ab.Caster.GetUnit().CbtInterface.GetTarget(GameData.TargetTypes.TARGETTYPES_TARGET_ENEMY);
            }

            if (Ab.Info.Entry == 1667) // Follo me
                CustomValue = GetAbilityDamage();

            if (Ab.Info.Entry == 1682) // Right in da jib
                CustomValue = GetAbilityDamage();

            if (Ab.Info.Entry == 1677) // Savin me hide
                CustomValue = GetAbilityDamage();

            if (Ab.Info.Entry == 1683) // Shut Yer Face
                CustomValue = GetAbilityDamage();

            if (Ab.Info.Entry == 1676) // Skull Thumper
                CustomValue = GetAbilityDamage();

            if (Ab.Info.Entry == 1670) // Trip Em Up
                CustomValue = GetAbilityDamage();

            if (Ab.Info.Entry == 1669) // Trip Em Up
                CustomValue = GetAbilityDamage();

            if (Ab.Info.Entry == 1666) // Wot Armor
                CustomValue = GetAbilityDamage();
        }

        public override void InitTargets(Unit Target)
        {
            this.Target = Target;
            CustomValue = 0;
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            GameData.AbilityResult Result = GameData.AbilityResult.ABILITYRESULT_OK;

            if (Target == null)
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET;
            else if (Target.IsDead)
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_DEAD;
            else if (!CombatInterface.CanAttack(Ab.Caster.GetUnit(), Target))
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_NOT_PVP_FLAGGED;
            else if (!CombatInterface.IsEnemy(Ab.Caster.GetUnit(), Target))
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_IN_YOUR_ALLIANCE;
            else if (IsStart)
            {
                float Distance = Ab.Caster.GetDistanceTo(Target);
                if (Distance > Ab.Info.Info.MaxRange)
                    Result = GameData.AbilityResult.ABILITYRESULT_OUTOFRANGE;
                else if (Distance < Ab.Info.Info.MinRange)
                    Result = GameData.AbilityResult.ABILITYRESULT_TOOCLOSE;
                else if (Ab.Info.Info.MaxRange <= 5 && !Ab.Caster.IsObjectInFront(Target, 110))
                    Result = GameData.AbilityResult.ABILITYRESULT_OUT_OF_ARC;
            }
            else
            {
                CalcualteStat(); // get Offensive/Defensive stat
                //Result = CheckBlock(); // check for parry, evade or disrupt.
            }

            return Result;
        }

        public override IAbilityTypeHandler Cast()
        {
            if (Target == null)
                return null;

            //Log.Info("DealDamage", "Cast");

            if (CustomValue == 0 && Ab.Info.GetTime(0) != 0 && Ab.Info.Info.ApSec == 0)
                return Target.AbtInterface.AddBuff(Ab, "DamageOverTime");

            if (CustomValue != 0 || Ab.Info.Info.ApSec != 0 || Ab.Info.GetTime(0) == 0 || Ab.Info.GetDamage(1) != 0)
            {
                uint Damage = GetAbilityDamage();
                CallOnCast(this, Damage);
                Ab.Caster.DealDamages(Target, Ab, Damage);
            }

            return null;
        }

        public override void SendDone()
        {
            if (Target != null)
            {
                Ab.SendAbilityDone(Target.Oid);
            }
        }

        public override void Stop()
        {
            SendDone();
            Target = null;
        }

        public override void Update(long Tick)
        {
            if (Target == null || Target.IsDead)
                Ab.Interface.Cancel(false);
        }

        public override uint GetAbilityDamage()
        {
            byte DamageType = (byte)Ab.Info.GetType(0);
            if (DamageType > 0)
                DamageType -= 1;
            
            double Damage = (float)Ab.Info.GetDamage(0) + Ab.Caster.ItmInterface.GetAttackDamage(EquipSlot.MAIN_DROITE);
            Log.Info("Damage+Init", ">>>>>> " + Damage);
            Damage += OffensiveStat / 5;
            Log.Info("Damage+Off", ">>>>>> " + Damage);
            Damage -= Target.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_TOUGHNESS) / 5;
            Log.Info("Damage-Def", ">>>>>> " + Damage);
            Damage *= (1 - Mitigation(DamageType));
            Log.Info("Damage Final", ">>>>>> " + Damage);
            return (uint)Damage;
        }

        public override void GetTargets(OnTargetFind OnFind)
        {
            OnFind(Target);
        }

        public float Mitigation(uint DamageType)
        {
            switch (DamageType)
            {
                default:
                case 0: //if attack is physical damage (doesn't matter if ranged or melee).
                    {
                        float SecondaryStat = Ab.Caster.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_WEAPONSKILL); //get attacker weaponskill.
                        float pen = SecondaryStat / (7.5f * Ab.Caster.Level + 50f) * 0.25f; //this will give you armour penetration value between 0 - 1, you add any bonus armour penetration from items to this. not included in formula for now. (just do +attackerbonus - defenderbonus).
                        float Armour = Target.ItmInterface.GetEquipedArmor(); //get defender armour
                        float mit = Armour / (Ab.Caster.Level * 44f) * 0.4f; //this will give you the total mitigation from armour.
                        mit *= 1f - pen;
                        if (mit > 0.75f) //puts in hard cap for physical mitigation of 75%
                        {
                            return 0.75f;
                        }
                        return mit;//this returns the % of damage mitigated by armour after armour penetration.
                    }
                case 1: //if attack is spirit
                    {
                        float SpiritResist = Target.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_SPIRITRESIST); // get defender spirit resist.
                        float mit = (SpiritResist / (Ab.Caster.Level * 8.4f)) * 0.2f; //calculate resistance mitigation
                        if (mit > 0.4f) //put in soft cap for resistances over 40%
                        {
                            return ((SpiritResist / (Ab.Caster.Level * 8.4f)) * 0.2f - 0.4f) / 3f + 0.4f; //basically, all resistances over 40% give a 1 point per 3.
                        }
                        return mit;
                    }
                case 2: //if attack is corporeal
                    {
                        float CorpResist = Target.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_CORPOREALRESIST); // get defender corp resist.
                        float mit = (CorpResist / (Ab.Caster.Level * 8.4f)) * 0.2f; //calculate resistance mitigation
                        if (mit > 0.4f) //put in soft cap for resistances over 40%
                        {
                            return ((CorpResist / (Ab.Caster.Level * 8.4f)) * 0.2f - 0.4f) / 3f + 0.4f; //basically, all resistances over 40% give a 1 point per 3.
                        }
                        return mit;
                    }
                case 3: //if attack is elemental
                    {
                        float ElemResist = Target.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_ELEMENTALRESIST); // get defender elemental resist.
                        float mit = (ElemResist / (Ab.Caster.Level * 8.4f)) * 0.2f; //calculate resistance mitigation
                        if (mit > 0.4f) //put in soft cap for resistances over 40%
                        {
                            return ((ElemResist / (Ab.Caster.Level * 8.4f)) * 0.2f - 0.4f) / 3f + 0.4f; //basically, all resistances over 40% give a 1 point per 3.
                        }
                        return mit;
                    }
            }
        }

        public void CalcualteStat()
        {
            int PrimaryStat = 0;

            uint AttackType = 0;
            if (Ab.Info.Info.MaxRange <= 5)
                AttackType = 0; // Melee
            else
                AttackType = 1; // Ranged

            switch ((GameData.CareerLine)Ab.Caster.GetPlayer()._Info.CareerLine)
            {
                case GameData.CareerLine.CAREERLINE_IRON_BREAKER:
                case GameData.CareerLine.CAREERLINE_SLAYER:
                case GameData.CareerLine.CAREERLINE_BLACK_ORC:
                case GameData.CareerLine.CAREERLINE_CHOPPA:
                case GameData.CareerLine.CAREERLINE_KNIGHT:
                case GameData.CareerLine.CAREERLINE_WARRIOR_PRIEST:
                case GameData.CareerLine.CAREERLINE_CHOSEN:
                case GameData.CareerLine.CAREERLINE_WARRIOR:
                case GameData.CareerLine.CAREERLINE_SHADE:
                case GameData.CareerLine.CAREERLINE_ASSASSIN:
                case GameData.CareerLine.CAREERLINE_BLOOD_PRIEST:
                case GameData.CareerLine.CAREERLINE_SEER:
                    PrimaryStat = 0;
                    break;

                case GameData.CareerLine.CAREERLINE_WITCH_HUNTER:
                case GameData.CareerLine.CAREERLINE_SWORDMASTER:
                case GameData.CareerLine.CAREERLINE_ENGINEER:
                    if (AttackType == 0)
                        PrimaryStat = 0;
                    else
                        PrimaryStat = 1;
                    break;

                case GameData.CareerLine.CAREERLINE_RUNE_PRIEST:
                case GameData.CareerLine.CAREERLINE_SHAMAN:
                case GameData.CareerLine.CAREERLINE_BRIGHT_WIZARD:
                case GameData.CareerLine.CAREERLINE_ZEALOT:
                case GameData.CareerLine.CAREERLINE_MAGUS:
                case GameData.CareerLine.CAREERLINE_ARCHMAGE:
                case GameData.CareerLine.CAREERLINE_SORCERER:
                    PrimaryStat = 2;
                    break;

                case GameData.CareerLine.CAREERLINE_SHADOW_WARRIOR:
                case GameData.CareerLine.CAREERLINE_SQUIG_HERDER:
                    if (AttackType == 0)
                        PrimaryStat = 0;
                    else if (Ab.Info.GetType(0) == 0)
                        PrimaryStat = 1;
                    else
                        PrimaryStat = 2;
                    break;
            }


            switch (PrimaryStat)
            {
                case 0: // Strength and Weaponskill
                    {
                        OffensiveStat = (uint)Ab.Caster.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_STRENGTH);
                        DefensiveStat = (uint)Target.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_WEAPONSKILL);
                    } break;
                case 1: // Ballistic and Initiative
                    {
                        OffensiveStat = (uint)Ab.Caster.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_BALLISTICSKILL);
                        DefensiveStat = (uint)Target.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_INITIATIVE);
                    } break;
                case 2: // Intelligence and Willpower
                    {
                        OffensiveStat = (uint)Ab.Caster.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_INTELLIGENCE);
                        DefensiveStat = (uint)Target.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_WILLPOWER);
                    } break;
            }
        }

        public GameData.AbilityResult CheckBlock()
        {
            // If wearing a shield
            int Block = 0;

            if (Target.ItmInterface.GetItemInSlot((UInt16)EquipSlot.MAIN_GAUCHE) != null && Target.ItmInterface.GetItemInSlot((UInt16)EquipSlot.MAIN_GAUCHE).Info.Type == 5)
            { 
                Block = (int)(((double)((float)Target.ItmInterface.GetItemInSlot((UInt16)EquipSlot.MAIN_GAUCHE).Info.Armor / OffensiveStat) * 0.2) * 100);
            }
            if (Block > 50)
                Block = 50;
            Log.Info("Block rate", "" + Block);
            Log.Info("GameData Block", "" +GameData.BonusTypes.BONUSTYPES_EBONUS_BLOCK);
            
            if (RandomMgr.Next(100) < (Block + (Target.StsInterface.GetTotalStat((byte)GameData.BonusTypes.BONUSTYPES_EBONUS_BLOCK) - Ab.Caster.StsInterface.GetTotalStat((byte)GameData.BonusTypes.BONUSTYPES_EBONUS_BLOCK_STRIKETHROUGH))))
                return GameData.AbilityResult.ABILITYRESULT_BLOCK;
            
            uint AttackType = 0;
            if (Ab.Info.Info.MaxRange <= 5)
                AttackType = 0; // Melee
            else
                AttackType = 1; // Ranged

            int SecondaryDefense = (int)((((double)DefensiveStat / OffensiveStat * 0.075) * 100));
            if (SecondaryDefense > 25)
                SecondaryDefense = 25;

            switch (AttackType)
            {
                case 0: // Parray
                    {
                        if (RandomMgr.Next(100) < (SecondaryDefense + Target.StsInterface.GetTotalStat((byte)GameData.BonusTypes.BONUSTYPES_EBONUS_PARRY) - Ab.Caster.StsInterface.GetTotalStat((byte)GameData.BonusTypes.BONUSTYPES_EBONUS_PARRY_STRIKETHROUGH))) // Parry
                            return GameData.AbilityResult.ABILITYRESULT_PARRY;
                    } break;
                case 1: // Evade
                    {
                        if (RandomMgr.Next(100) < (SecondaryDefense + Target.StsInterface.GetTotalStat((byte)GameData.BonusTypes.BONUSTYPES_EBONUS_EVADE) - Ab.Caster.StsInterface.GetTotalStat((byte)GameData.BonusTypes.BONUSTYPES_EBONUS_EVADE_STRIKETHROUGH))) // Dodge
                            return GameData.AbilityResult.ABILITYRESULT_EVADE;
                    } break;
                case 2: // Disrupt
                    {
                        if (RandomMgr.Next(100) < (SecondaryDefense + Target.StsInterface.GetTotalStat((byte)GameData.BonusTypes.BONUSTYPES_EBONUS_DISRUPT) - Ab.Caster.StsInterface.GetTotalStat((byte)GameData.BonusTypes.BONUSTYPES_EBONUS_DISRUPT_STRIKETHROUGH))) // Disrupt
                            return GameData.AbilityResult.ABILITYRESULT_DISRUPT;
                    } break;
            }

            Log.Info("Block", "<<<<<<<<<<<<<<<");
            return GameData.AbilityResult.ABILITYRESULT_OK;
        }
    }
}
