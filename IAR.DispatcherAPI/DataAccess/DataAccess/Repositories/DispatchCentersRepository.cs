using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities;

namespace DataAccess.Repositories
{
    public class DispatchCentersRepository : EntityBaseRepository<DispatchCenters>, IDispatchCentersRepository
    {
        public DispatchCentersRepository(DispatchApidbContext context) : base(context)
        {
        }
    }
}
