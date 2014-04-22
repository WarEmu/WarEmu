
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public enum EventName
    {
        ON_CREATE,
        ON_LOAD_START,
        ON_LOAD_FINISH,

        ON_FIRST_ENTER_WORLD,
        ON_ENTER_WORLD,
        ON_REMOVE_FROM_WORLD,

        ON_MOVE,
        ON_STOP_MOVE,
        ON_CHANGE_OFFSET,
        ON_CHANGE_ZONE,

        ON_ENTER_COMBAT,
        ON_DEAL_DAMAGE,
        ON_RECEIVE_DAMAGE,
        ON_DEAL_HEAL,
        ON_RECEIVE_HEAL,
        ON_TARGET_DIE,
        ON_LEAVE_COMBAT,

        ON_REZURECT,
        ON_DIE,

        ON_GENERATE_LOOT, // Sender = Killer, Obj = Loot()

        ON_ACCEPT_QUEST, // Sender = Player, Obj = Character_Quest
        ON_DECLINE_QUEST,// Sender = Player, Obj = Character_Quest
        ON_DONE_QUEST, 
        ON_ABORD_QUEST, // Sender = Player, Obj = Character_Quest

        PLAYING,
        LEAVE,
        ON_LEVEL_UP,

        ARRIVE_AT_TARGET,
        ON_WALK_TO,
        ON_WALK,

        ON_START_CASTING,
        ON_CAST,
    };

    public delegate void EventDelegate();

    public class EventInfo
    {
        public int Interval;
        public int Count;
        public int BaseCount;
        public EventDelegate Del;
        private long NextExecute = 0;
        public bool ToDelete = false;

        public EventInfo(EventDelegate Del,int Interval, int Count)
        {
            this.Del = Del;
            this.Interval = Interval;
            this.Count = Count;
            this.BaseCount = Count;

            if (Interval == 0 )
                ToDelete = true;

            //Log.Success("AddEvent", "Del =" + Del.Method.Name + ",Name" + Del.Target.ToString());
        }

        public bool Update(long Tick)
        {
            if (ToDelete)
                return true;

            if (NextExecute == 0)
                NextExecute = Tick + Interval;

            if (NextExecute <= Tick)
            {
                if(BaseCount > 0)
                    --Count;

                if (Del != null)
                    Del.Invoke();

                NextExecute = Tick + Interval;
            }

            if (Count <= 0 && this.BaseCount != 0)
                ToDelete = true;

            return ToDelete;
        }
    }

    public class EventInterface : BaseInterface
    {
        static public Dictionary<uint, EventInterface> _EventInterfaces = new Dictionary<uint, EventInterface>();

        static public EventInterface GetEventInterface(uint CharacterId)
        {
            lock (_EventInterfaces)
            {
                if (!_EventInterfaces.ContainsKey(CharacterId))
                    _EventInterfaces.Add(CharacterId, new EventInterface());

                return _EventInterfaces[CharacterId];
            }
        }

        public delegate bool EventNotify(Object Obj, object Args); // True si il doit être delete apres la notificatio

        public EventInterface()
            : base()
        {

        }

        public override void Stop()
        {
            Running = false;

            lock (_Events)
                _Events.Clear();

            lock (_Notify)
                _Notify.Clear();

            base.Stop();
        }

        public void Start()
        {
            Running = true;
        }

        #region Events

        private bool Running = true;
        public List<EventInfo> _Events = new List<EventInfo>();
        public override void Update(long Tick)
        {
            if (!Running)
                return;

            lock (_Events)
                _Events.RemoveAll(Info => !Running || Info.Update(Tick));
        }
        public void AddEvent(EventDelegate Del, int Interval, int Count)
        {
            lock (_Events)
                _Events.Add(new EventInfo(Del, Interval, Count));
        }
        public void RemoveEvent(EventDelegate Del)
        {
            lock (_Events)
                _Events.RemoveAll(Info => Info.Del == Del);
        }
        public bool HasEvent(EventDelegate Del)
        {
            lock (_Events)
                return _Events.Find(info => info.Del == Del) != null;
        }
        #endregion

        #region Notify

        public Dictionary<string,List<EventNotify>> _Notify = new Dictionary<string,List<EventNotify>>();
        public Dictionary<string, List<EventNotify>> _ForceNotify = new Dictionary<string, List<EventNotify>>();

        public void Notify(EventName Name, Object Sender, object Args)
        {
           // Log.Info("Notify", "[" + (Obj != null ? Obj.Name : "") + "] Appel de :" + EventName);

            List<EventNotify> L;
            lock (_Notify)
            {
                if (_Notify.TryGetValue(Name.ToString(), out L))
                    L.RemoveAll(Event => Event.Invoke(Sender, Args));
            }

            lock (_ForceNotify)
            {
                if (_ForceNotify.TryGetValue(Name.ToString(), out L))
                    L.RemoveAll(Event => Event.Invoke(Sender, Args));
            }
        }

        public void AddEventNotify(EventName Name, EventNotify Event)
        {
            AddEventNotify(Name, Event, false);
        }
        public void AddEventNotify(EventName Name, EventNotify Event, bool Force)
        {
            //if(_Owner.IsPlayer())
            //Log.Info("AddEventNotify", "[" + (_Owner != null ? _Owner.Name : "") + "] Add de :" + EventName);
            
            List < EventNotify > L;
            if (!Force)
            {
                lock (_Notify)
                {
                    if (!_Notify.TryGetValue(Name.ToString(), out L))
                    {
                        L = new List<EventNotify>();
                        _Notify.Add(Name.ToString(), L);
                    }

                    if (!L.Contains(Event))
                        L.Add(Event);
                }
            }
            else
            {
                lock (_ForceNotify)
                {
                    if (!_ForceNotify.TryGetValue(Name.ToString(), out L))
                    {
                        L = new List<EventNotify>();
                        _ForceNotify.Add(Name.ToString(), L);
                    }

                    if (!L.Contains(Event))
                        L.Add(Event);
                }
            }
        }
        public void RemoveEventNotify(EventName Name, EventNotify Event)
        {
            List<EventNotify> L;
            lock (_Notify)
            {
                if (_Notify.TryGetValue(Name.ToString(), out L))
                    L.Remove(Event);
            }

            lock (_ForceNotify)
            {
                if (_ForceNotify.TryGetValue(Name.ToString(), out L))
                    L.Remove(Event);
            }
        }

        #endregion


    }
}
