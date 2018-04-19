using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class SubscriberByOriginationDestinationRepository : EntityBaseRepository<SubscriberByOriginationDestination>, ISubscriberByOriginationDestinationRepository
    {
        public SubscriberByOriginationDestinationRepository(DispatchApidbContext context) : base(context)
        {
      
        }
    }
}
