
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace LauncherServer
{
    public class TCPServer : TCPManager
    {
        public TCPServer()
            : base()
        {

        }

        protected override BaseClient GetNewClient()
        {
            Client client = new Client(this);

            return client;
        }
    }
}
