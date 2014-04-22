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
using System.Reflection;

using FrameWork;
using Common;

namespace WorldServer
{
    static public class AbilityMgr
    {
        static public MySQLObjectDatabase Database;

        static public Dictionary<string, Type> _AbilityTypes = new Dictionary<string, Type>();
        static public Dictionary<ushort, Type> _AbilityTypesEntry = new Dictionary<ushort, Type>();
        static public Dictionary<byte, List<Ability_Info>> _CareerAbility = new Dictionary<byte, List<Ability_Info>>();

        static public void LoadAbilityHandlers()
        {
            _AbilityTypes = new Dictionary<string, Type>();

            Log.Debug("AbilityMgr", "Loading Ability Type Handlers...");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;

                    if (!type.IsSubclassOf(typeof(IAbilityTypeHandler)))
                        continue;

                    IAbilityTypeAttribute[] attrib = type.GetCustomAttributes(typeof(IAbilityTypeAttribute), true) as IAbilityTypeAttribute[];
                    if (attrib.Length <= 0)
                        continue;

                    Log.Info("AbilityMgr", "Registering Ability Type : " + attrib[0].TypeDescription +"("+attrib[0].HandlerName+")");

                    if (attrib[0].AbilityEntry != null)
                    {
                        foreach (ushort Id in attrib[0].AbilityEntry)
                        {
                            if (!_AbilityTypesEntry.ContainsKey(Id))
                                _AbilityTypesEntry.Add(Id, type);
                            else
                                _AbilityTypesEntry[Id] = type;
                        }
                    }
                    
                    if(attrib[0].HandlerName != null && attrib[0].HandlerName.Length > 0)
                    {
                        if (!_AbilityTypes.ContainsKey(attrib[0].HandlerName))
                            _AbilityTypes.Add(attrib[0].HandlerName, type);
                        else
                            _AbilityTypes[attrib[0].HandlerName] = type;
                    }

                }
            }
        }

        static public bool HasAbilityHandler(ushort Entry, string HandlerName)
        {
            return _AbilityTypes.ContainsKey(HandlerName) || _AbilityTypesEntry.ContainsKey(Entry);
        }

        static public IAbilityTypeHandler GetAbilityHandler(ushort Entry, string HandlerName)
        {
            Type Handler = null;
            if (_AbilityTypesEntry.TryGetValue(Entry, out Handler))
                return Activator.CreateInstance(Handler) as IAbilityTypeHandler;

            if (_AbilityTypes.TryGetValue(HandlerName, out Handler))
                return Activator.CreateInstance(Handler) as IAbilityTypeHandler;

            return null;
        }

        #region Ability_Info

        static public Dictionary<UInt16, List<Ability_Info>> _AbilityInfos = new Dictionary<ushort, List<Ability_Info>>();
        static public Dictionary<UInt16, List<Ability_Stats>> _AbilityStats = new Dictionary<ushort, List<Ability_Stats>>();

        [LoadingFunction(true)]
        static public void LoadAbilityInfo()
        {
            LoadAbilityHandlers();

            _AbilityInfos = new Dictionary<ushort, List<Ability_Info>>();

            Log.Debug("AbilityMgr", "Loading Ability Info...");

            IList<Ability_Info> Infos = Database.SelectAllObjects<Ability_Info>();
            IList<Ability_Stats> Stats = Database.SelectAllObjects<Ability_Stats>();
            List<Ability_Stats> S;

            foreach (Ability_Stats Stat in Stats)
            {
                if (!_AbilityStats.TryGetValue(Stat.Entry, out S))
                {
                    S = new List<Ability_Stats>();
                    _AbilityStats.Add(Stat.Entry, S);
                }

                S.Add(Stat);
            }

            int Error = 0;

            foreach (Ability_Info Info in Infos)
            {
                if (Info.Entry == 245)
                    Info.CareerLine = 0;

                Info.Stats = _AbilityStats[Info.Entry];
                foreach (Ability_Stats Stat in Info.Stats)
                {
                    Stat.Info = Info;

                    if (Stat.GetRadius(0) > 20 && Info.HandlerName.ToLower().Contains("zone"))
                    {
                        Log.Info("AbilityMgr", "Disabled Ability : " + Info.Entry + "," + Info.Name + ", Range=" + Stat.GetRadius(0) + ",Damages=" + Stat.GetDamage(0));
                        Info.HandlerName = "";
                    }
                }

                if (Info.HandlerName == null)
                    Info.HandlerName = "";

                if (Info.EffectID == 0 && Info.CastTime != 0)
                    Info.EffectID = 100;

                if (!HasAbilityHandler(Info.Entry, Info.HandlerName))
                {
                    Log.Debug("AbilityMgr", "Ability Type of : " + Info.Entry + " do not exist : " + Info.HandlerName);
                    ++Error;
                }


                if (!_AbilityInfos.ContainsKey(Info.Entry))
                    _AbilityInfos.Add(Info.Entry, new List<Ability_Info>());

                _AbilityInfos[Info.Entry].Add(Info);

                if (Info.CareerLine != 0)
                {
                    if (!_CareerAbility.ContainsKey(Info.CareerLine))
                        _CareerAbility.Add(Info.CareerLine, new List<Ability_Info>());

                    _CareerAbility[Info.CareerLine].Add(Info);
                }
            }

            foreach (Ability_Info Info in Infos)
            {
                if (Info.CareerLine == 0)
                {
                    foreach (KeyValuePair<byte, List<Ability_Info>> Kp in _CareerAbility)
                        Kp.Value.Add(Info);
                }
            }

            if (Error > 0)
                Log.Error("AbilityMgr", "[" + Error + "] Ability Error");

            Log.Success("AbilityMgr", "Loaded " + _AbilityInfos.Count + " Ability Info");
        }

        static public List<Ability_Info> GetCareerAbility(byte CareerLine)
        {
            List<Ability_Info> L;
            _CareerAbility.TryGetValue(CareerLine, out L);
            return L;
        }

        #endregion
    }
}
