using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities;

namespace DataAccess.Repositories
{
    public class DispatchUnmatchedMessagesRepository : EntityBaseRepository<DispatchUnmatchedMessages>, IDispatchUnmatchedMessagesRepository
    {
        public DispatchUnmatchedMessagesRepository(DispatchApidbContext context) : base(context)
        {
        }
    }
}
