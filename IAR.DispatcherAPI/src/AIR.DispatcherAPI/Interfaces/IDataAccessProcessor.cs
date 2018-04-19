using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace IAR.DispatcherAPI.Interfaces
{
    public interface IDataAccessProcessor
    {
        Subscriber GetSubscriptor(int id);
        Task<List<SubscriberByOriginationDestination>> GetSubscriberByOriginationDestination(string origination,
            string destination);
        Task<DispatchMessage> GetLatestDispatchMessageForSubscriber(int subscriberId);
        DispatchMessage SaveMessage(DispatchMessage message);
        Task<List<SubscriberInfoBySubscriberIds>> GetSubscriberInfoBySubscriberIdsInUnmatchedDispatch(string subscriberIds);
    }
}
