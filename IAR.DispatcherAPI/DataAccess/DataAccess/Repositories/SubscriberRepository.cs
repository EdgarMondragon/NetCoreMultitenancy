using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities;

namespace DataAccess.Repositories
{
    public class SubscriberRepository : EntityBaseRepository<Subscriber>, ISubscriberRepository
    {
        public SubscriberRepository(DispatchApidbContext context) : base(context)
        {
        }
    }
}
