
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

        static public Dictionary<UInt16, IAbilityTypeHandler> _AbilityTypes = new Dictionary<ushort, IAbilityTypeHandler>();
        static public Dictionary<byte, List<Ability_Info>> _CareerAbility = new Dictionary<byte, List<Ability_Info>>();

        static private void LoadAbilityType()
        {
            _AbilityTypes = new Dictionary<ushort, IAbilityTypeHandler>();

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

                    Log.Info("AbilityMgr", "Registering Ability Type : " + attrib[0].TypeDescription +"("+attrib[0].AbilityType+")");
                    _AbilityTypes.Add(attrib[0].AbilityType, (IAbilityTypeHandler)Activator.CreateInstance(type));
                }
            }
        }

        static public bool HasAbilityHandler(UInt16 AbilityType)
        {
            return _AbilityTypes.ContainsKey(AbilityType);
        }

        static public IAbilityTypeHandler GetAbilityHandler(UInt16 AbilityType)
        {
            IAbilityTypeHandler Handler = null;
            if (_AbilityTypes.TryGetValue(AbilityType, out Handler))
                return Activator.CreateInstance(Handler.GetType()) as IAbilityTypeHandler;

            return null;
        }

        #region Ability_Info

        static public Dictionary<UInt16, List<Ability_Info>> _AbilityInfos = new Dictionary<ushort, List<Ability_Info>>();

        [LoadingFunction(true)]
        static public void LoadAbilityInfo()
        {
            LoadAbilityType();

            _AbilityInfos = new Dictionary<ushort, List<Ability_Info>>();

            Log.Debug("AbilityMgr", "Loading Ability Info...");

            IList<Ability_Info> Infos = Database.SelectAllObjects<Ability_Info>();
            int Error = 0;

            foreach (Ability_Info Info in Infos)
            {
                if (!HasAbilityHandler(Info.AbilityType))
                {
                    Log.Debug("AbilityMgr", "Ability Type of : " + Info.Entry + " do not exist : " + Info.AbilityType);
                    ++Error;
                    continue;
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

            if (Error > 0)
                Log.Error("AbilityMgr", "[" + Error + "] Ability Error");

            Log.Success("AbilityMgr", "Loaded " + _AbilityInfos.Count + " Ability Info");
        }

        static public Ability_Info GetAbilityInfo(UInt16 AbilityEntry,byte Level)
        {
            Ability_Info Info = null;
            if (_AbilityInfos.ContainsKey(AbilityEntry))
                Info = _AbilityInfos[AbilityEntry].Find(info => info.Level == Level);
            return Info;
        }

        static public List<Ability_Info> GetCareerAbility(byte CareerLine, byte Level)
        {
            List<Ability_Info> Abilities = new List<Ability_Info>();
            if (_CareerAbility.ContainsKey(CareerLine))
                Abilities.AddRange(_CareerAbility[CareerLine].FindAll(info => info.Level <= Level));
            return Abilities;
        }

        #endregion
    }
}
