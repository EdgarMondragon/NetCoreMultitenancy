using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IAR.DispatcherAPI.Interfaces;
using Entities;
using DataAccess.Abstract;
using NLog;

namespace IAR.DispatcherAPI.Methods
{
    public class DataAccessProcessor : IDataAccessProcessor
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriberByOriginationDestinationRepository _subscriberByOriginationDestinationRepository;
        private readonly IDispatchMessageRepository _dispatchMessageRepository;

        private readonly ISubscriberInfoBySubscriberIdsInUnmatchedDispatchRepository
            _subscriberInfoBySubscriberIdsInUnmatchedDispatchRepository;
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public DataAccessProcessor(ISubscriberRepository subscriberRepository,
            ISubscriberByOriginationDestinationRepository subscriberByOriginationDestinationRepository, IDispatchMessageRepository dispatchMessageRepository,
            ISubscriberInfoBySubscriberIdsInUnmatchedDispatchRepository subscriberIdsInUnmatchedDispatchRepository)
        {
            _subscriberRepository = subscriberRepository;
            _subscriberByOriginationDestinationRepository = subscriberByOriginationDestinationRepository;
            _dispatchMessageRepository = dispatchMessageRepository;
            _subscriberInfoBySubscriberIdsInUnmatchedDispatchRepository = subscriberIdsInUnmatchedDispatchRepository;
        }

        public Subscriber GetSubscriptor(int id)
        {
            try
            {
                var subscriber = _subscriberRepository.FindAsync(s => s.Id == id);

                return subscriber.Result;
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<SubscriberByOriginationDestination>> GetSubscriberByOriginationDestination(string origination, string destination)
        {
            try
            {
                string sqlQuery = string.Format("Call GetSubscriberByOriginationDestination('{0}', '{1}')", origination, destination);

                var result = await Task.Run(() => _subscriberByOriginationDestinationRepository.ExecWithStoreProcedure(sqlQuery));

                return result.ToList();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public async Task<DispatchMessage> GetLatestDispatchMessageForSubscriber(int subscriberId)
        {
            try
            {
                string sqlQuery = string.Format("Call GetLatestDispatchMessageForSubscriber('{0}')", subscriberId);

                var result = await Task.Run(() => _dispatchMessageRepository.ExecWithStoreProcedure(sqlQuery));

                return result.FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public DispatchMessage SaveMessage(DispatchMessage message)
        {
            return _dispatchMessageRepository.AddAsync(message).Result;
        }

        public async Task<List<SubscriberInfoBySubscriberIds>> GetSubscriberInfoBySubscriberIdsInUnmatchedDispatch(string subscriberIds)
        {
            try
            {
                string sqlQuery = string.Format("Call GetSubscriberInfoBySubscriberIdsInUnmatchedDispatch('{0}')", subscriberIds);

                var result = await Task.Run(() => _subscriberInfoBySubscriberIdsInUnmatchedDispatchRepository.ExecWithStoreProcedure(sqlQuery));

                return result.ToList();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }
    }
}
