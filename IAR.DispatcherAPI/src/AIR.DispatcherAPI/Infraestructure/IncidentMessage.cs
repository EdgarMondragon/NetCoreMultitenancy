using System;
using Entities;

namespace IAR.DispatcherAPI.Infraestructure
{
    public class IncidentMessage
    {

        public int Id { get; set; }
        public DateTime ArrivedOn { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string DestinationEmailAddress { get; set; }
        public string OriginationEmailAddress { get; set; }
        public int SubscriberId { get; set; }
        public object Subject { get; set; }
        public bool DispatchMessageAPI { get; set; }

        public static IncidentMessage FromDispatchMessage(DispatchMessage incidentMessage)
        {
            return new IncidentMessage
            {
                Id = incidentMessage.Id,
                ArrivedOn = incidentMessage.Arrivedon,
                Header = incidentMessage.Messageheader,
                Body = incidentMessage.Messagebody,
                DestinationEmailAddress = incidentMessage.Destinationemailaddress,
                OriginationEmailAddress = incidentMessage.Originationemailaddress,
                SubscriberId = incidentMessage.Subscriberid,
                Subject = incidentMessage.Messagesubject,
                DispatchMessageAPI = true
                
            };
        }
    }
}