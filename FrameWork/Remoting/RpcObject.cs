
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting;

namespace FrameWork
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RpcAttribute : Attribute
    {
        public bool ForServer;
        public WellKnownObjectMode Mode;
        public int[] AllowedID;

        public RpcAttribute(bool ForServer, WellKnownObjectMode Mode, params int[] AllowedID)
        {
            this.ForServer = ForServer;
            this.Mode = Mode;
            this.AllowedID = AllowedID;
        }
    }


    public class RpcObject : MarshalByRefObject
    {
        public RpcClientInfo MyInfo;

        public virtual void OnClientConnected(RpcClientInfo Info)
        {

        }

        public virtual void OnClientDisconnected(RpcClientInfo Info)
        {

        }

        public virtual void OnServerConnected()
        {

        }

        public virtual void OnServerDisconnected()
        {

        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        static public List<Type>[] RegisterHandlers(bool ForServer, int AllowedID)
        {
            Log.Debug(ForServer ? "RpcServer" : "RpcClient", "Registering handlers");

            List<Type>[] Registered = new List<Type>[2] { new List<Type>(), new List<Type>() };

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = null;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }

                foreach (Type type in types)
                {
                    if (type == null || !type.IsClass || !type.IsSubclassOf(typeof(RpcObject)))
                        continue;

                    RpcAttribute[] attrib = type.GetCustomAttributes(typeof(RpcAttribute), true) as RpcAttribute[];
                    if (attrib.Length <= 0)
                        continue;

                    if (attrib[0].AllowedID.ToList().Contains(AllowedID) || attrib[0].AllowedID.ToList().Contains(0))
                    {
                        if (attrib[0].ForServer == ForServer)
                        {
                            Log.Debug(ForServer ? "RpcServer" : "RpcClient", "Registering : " + type.Name);
                            RemotingConfiguration.RegisterWellKnownServiceType(type, type.Name, attrib[0].Mode);
                        }

                        if (attrib[0].ForServer)
                            Registered[0].Add(type);
                        else
                            Registered[1].Add(type);
                    }
                }
            }

            return Registered;
        }
    }
}
