using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class DispatchUnmatchedMessages : IEntityBase
    {
        public int Id { get; set; }
        public DateTime Arrivedon { get; set; }
        public string Messageheader { get; set; }
        public string Messagesubject { get; set; }
        public string Messagebody { get; set; }
        public string Messagefrom { get; set; }
        public string Messageto { get; set; }
        public int Destsubscriberid { get; set; }
        public string Destsubscribername { get; set; }
        public int Destsubscriberstatusid { get; set; }
        public string Destsubscriberstatusname { get; set; }
    }
}
