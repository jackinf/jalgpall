using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uptime_Jalgpall.Models
{
    public class Pair
    {
        public int ID { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public Tournament Tournament { get; set; }
        public int Team1Scored { get; set; }
        public int Team2Scored { get; set; }

    }
}