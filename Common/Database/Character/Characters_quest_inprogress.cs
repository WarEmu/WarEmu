using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    public class Character_InProgressObjectives
    {
        public Quest_Objectives Objective;
        public int ObjectiveID;
        public int Count;

        public bool IsDone()
        {
            if (Objective == null)
                return false;
            else
                return Count >= Objective.ObjCount;
        }
    }
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "Characters_quests_inprogress", DatabaseName = "Characters")]
    [Serializable]
    public class Character_quest_inprogress : DataObject
    {
        [DataElement(AllowDbNull = false)]
        public int CharacterID;

        [DataElement(AllowDbNull = false)]
        public UInt16 QuestID;

        [DataElement(AllowDbNull = false)]
        public string Objectives
        {
            get
            {
                string Value = "";
                foreach (Character_InProgressObjectives Obj in _InProgressObjectives)
                    Value += Obj.ObjectiveID + ":" + Obj.Count + "|";
                return Value;
            }
            set
            {
                if (value.Length <= 0)
                    return;

                string[] Objs = value.Split('|');

                foreach (string Obj in Objs)
                {
                    if (Obj.Length <= 0)
                        continue;

                    int ObjectiveID = int.Parse(Obj.Split(':')[0]);
                    int Count = int.Parse(Obj.Split(':')[1]);

                    Character_InProgressObjectives CObj = new Character_InProgressObjectives();
                    CObj.ObjectiveID = ObjectiveID;
                    CObj.Count = Count;
                    _InProgressObjectives.Add(CObj);
                }
            }
        }

        public List<Character_InProgressObjectives> _InProgressObjectives = new List<Character_InProgressObjectives>();

        public Quest Quest;

    }
}
