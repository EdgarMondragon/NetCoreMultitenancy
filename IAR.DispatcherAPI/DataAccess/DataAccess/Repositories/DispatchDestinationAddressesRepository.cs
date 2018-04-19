using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities;

namespace DataAccess.Repositories
{
    public class DispatchDestinationAddressesRepository : EntityBaseRepository<DispatchDestinationAddresses>, IDispatchDestinationAddressesRepository
    {
        public DispatchDestinationAddressesRepository(DispatchApidbContext context) : base(context)
        {
        }
    }
}
