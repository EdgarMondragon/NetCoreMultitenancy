using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class SubscriberInfoBySubscriberIds : IEntityBase
    {
        public int Id{get; set; }
        public string Subscribername { get; set; }
        public int Statusid { get; set; }
        public string Statusname { get; set; }
        public string Loginname { get; set; }
        public int Mailingcountry { get; set; }
        public string Mailingstate { get; set; }
        public string Subscribertypeid { get; set; }
        public string Fullname { get; set; }
    }
}
