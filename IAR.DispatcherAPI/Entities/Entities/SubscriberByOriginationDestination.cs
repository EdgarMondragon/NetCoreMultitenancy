using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class SubscriberByOriginationDestination : IEntityBase
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
    }
}
