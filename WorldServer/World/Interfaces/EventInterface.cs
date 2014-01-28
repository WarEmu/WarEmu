
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
        ON_HEAL,
        ON_REVEIVE_HEAL,
        ON_LEAVE_COMBAT,

        ON_REZURECT,
        ON_DIE,

        ON_ACCEPT_QUEST,
        ON_DECLINE_QUEST,
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

        public delegate bool EventNotify(Object Obj,EventArgs Args); // True si il doit être delete apres la notification

        public EventInterface()
        {

        }

        public EventInterface(Object Obj)
            : base(Obj)
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

        #endregion

        #region Notify

        public Dictionary<string,List<EventNotify>> _Notify = new Dictionary<string,List<EventNotify>>();
        public Dictionary<string, List<EventNotify>> _ForceNotify = new Dictionary<string, List<EventNotify>>();

        public void Notify(string EventName,Object Sender, EventArgs Args)
        {
           // Log.Info("Notify", "[" + (Obj != null ? Obj.Name : "") + "] Appel de :" + EventName);

            lock (_Notify)
                if (_Notify.ContainsKey(EventName))
                    _Notify[EventName].RemoveAll(Event => Event.Invoke(Sender, Args));

            lock (_ForceNotify)
                if (_ForceNotify.ContainsKey(EventName))
                    _ForceNotify[EventName].RemoveAll(Event => Event.Invoke(Sender, Args));
        }

        public void AddEventNotify(string EventName,EventNotify Event)
        {
            AddEventNotify(EventName, Event, false);
        }
        public void AddEventNotify(string EventName,EventNotify Event,bool Force)
        {
            Log.Info("AddEventNotify", "[" + (Obj != null ? Obj.Name : "") + "] Add de :" + EventName);
            if (!Force)
            {
                lock (_Notify)
                {
                    if (!_Notify.ContainsKey(EventName))
                        _Notify.Add(EventName, new List<EventNotify>());

                    if (!_Notify[EventName].Contains(Event))
                        _Notify[EventName].Add(Event);
                }
            }
            else
            {
                lock (_ForceNotify)
                {
                    if (!_ForceNotify.ContainsKey(EventName))
                        _ForceNotify.Add(EventName, new List<EventNotify>());

                    if (!_ForceNotify[EventName].Contains(Event))
                        _ForceNotify[EventName].Add(Event);
                }
            }
        }
        public void RemoveEventNotify(string EventName, EventNotify Event)
        {
            lock (_Notify)
                if (_Notify.ContainsKey(EventName))
                    _Notify[EventName].Remove(Event);

            lock (_ForceNotify)
                if (_ForceNotify.ContainsKey(EventName))
                    _ForceNotify[EventName].Remove(Event);
        }

        #endregion


    }
}
