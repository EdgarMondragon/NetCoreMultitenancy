using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using DataAccess.Abstract;
using Entities;

namespace BusinessLogic
{
    public class BusinessLogic
    {
        private static ISubscriberByOriginationDestinationRepository _subscriberByOriginationDestinationRepository;
        private static ISubscriberRepository _subscriberRepository;

        public BusinessLogic(ISubscriberByOriginationDestinationRepository subscriberByOriginationDestinationRepository, ISubscriberRepository subscriberRepository)
        {
            _subscriberByOriginationDestinationRepository = subscriberByOriginationDestinationRepository;
            _subscriberRepository = subscriberRepository;
        }

        public static List<SubscriberByOriginationDestination> GetSubscriberByOriginationDestination(string origination,
            string destination)
        {
            string sqlQuery = "GetSubscriberByOriginationDestination {0}, {1}";

            object[] sqlParams = {
                    new SqlParameter("p_Origination", origination),
                    new SqlParameter("p_Destination", destination)
            };

            return _subscriberByOriginationDestinationRepository.ExecWithStoreProcedure(sqlQuery, sqlParams).ToList();

        }

        public static Subscriber GetSubscriber(int id)
        {
            Subscriber subscriber = _subscriberRepository.GetSingle(s => s.Id == id);

            return subscriber;
        }

    }
}
