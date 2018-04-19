using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class DispatchCenters : IEntityBase
    {
        public int Id { get; set; }
        public string ApiKey { get; set; }
        public string DispatcherMasterName { get; set;}
        public string DispatcherUserPassword { get; set; }
    }
}
