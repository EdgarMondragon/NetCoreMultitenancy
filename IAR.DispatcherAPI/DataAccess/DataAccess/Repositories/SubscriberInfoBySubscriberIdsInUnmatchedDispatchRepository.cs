using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities;

namespace DataAccess.Repositories
{
    public class SubscriberInfoBySubscriberIdsInUnmatchedDispatchRepository : EntityBaseRepository<SubscriberInfoBySubscriberIds>, ISubscriberInfoBySubscriberIdsInUnmatchedDispatchRepository
    {
        public SubscriberInfoBySubscriberIdsInUnmatchedDispatchRepository(DispatchApidbContext context) : base(context)
        {

        }
    }
}
