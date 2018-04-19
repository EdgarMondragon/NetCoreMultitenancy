using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;

namespace DataAccess.Abstract
{
    public interface ISubscriberRepository : IEntityBaseRepository<Subscriber> { }
    public interface IDispatchMessageRepository : IEntityBaseRepository<DispatchMessage>{ }
    public interface ISubscriberByOriginationDestinationRepository : IEntityBaseRepository<SubscriberByOriginationDestination>{}
    public interface IDispatchDestinationAddressesRepository : IEntityBaseRepository<DispatchDestinationAddresses>{}
    public interface IDispatchUnmatchedMessagesRepository : IEntityBaseRepository<DispatchUnmatchedMessages>{}
    public interface ISubscriberInfoBySubscriberIdsInUnmatchedDispatchRepository : IEntityBaseRepository<SubscriberInfoBySubscriberIds>{}
    public interface IDispatchCentersRepository : IEntityBaseRepository<DispatchCenters> { }
}
