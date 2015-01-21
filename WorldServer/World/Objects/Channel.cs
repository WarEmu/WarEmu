using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldServer
{
    public class Channel
    {
        public String Name { get; set; }
        public List<Player> Members {get; set;}
        public byte Faction { get; set; }
        public Boolean IsPublic { get; set; }
    }
}
