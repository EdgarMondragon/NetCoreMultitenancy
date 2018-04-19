using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class DispatchDestinationAddresses : IEntityBase
    {
        public int Id { get; set;}
        public int Subscriberid { get; set; }
        public string Emailaddress { get; set; }
        public string Name { get; set; }
        public int cr_isdeleted { get; set; }
        public DateTime cr_lastupdated { get; set; }
    }
}
