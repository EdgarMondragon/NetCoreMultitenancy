using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class Subscriber : IEntityBase
    {
        public int Id { get; set; }
        public string Subscribername { get; set; }
        public bool Usedispatchfromattachment { get; set; }
       
    }
}
