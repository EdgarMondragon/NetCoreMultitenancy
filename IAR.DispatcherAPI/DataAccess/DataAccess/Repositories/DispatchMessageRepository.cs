using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities;

namespace DataAccess.Repositories
{
    public class DispatchMessageRepository : EntityBaseRepository<DispatchMessage>, IDispatchMessageRepository
    {
        public DispatchMessageRepository(DispatchApidbContext context) : base(context)
        {
         
        }
    }
}
