using System;
using Newtonsoft.Json;

namespace IAR.DispatcherAPI.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Message
    {
        [JsonProperty(PropertyName = "ID")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "ArrivedOn")]
        public DateTime Arrivedon { get; set; }

        [JsonProperty(PropertyName = "MessageHeader")]
        public string Messageheader { get; set; }

        [JsonProperty(PropertyName = "MessageBody")]
        public string Messagebody { get; set; }

        [JsonProperty(PropertyName = "SubscriberID")]
        public int Subscriberid { get; set; }

        [JsonProperty(PropertyName = "DestinationEmailAddress")]
        public string Destinationemailaddress { get; set; }

        [JsonProperty(PropertyName = "OriginationEmailAddress")]
        public string Originationemailaddress { get; set; }

        [JsonProperty(PropertyName = "Subject")]
        public string Messagesubject { get; set; }
        //public int? Recentduplication { get; set; }
        //public int? Screenmsgduplicated { get; set; }
        //public int? Emailmsgduplicated { get; set; }
        //public int? Textmsgduplicated { get; set; }
        //public int? Dscreenmsgduplicated { get; set; }

        [JsonProperty(PropertyName = "XSender")]
        public string XSender { get; set; }

        [JsonProperty(PropertyName = "XReceiver")]
        public string XReceiver { get; set; }

        [JsonProperty(PropertyName = "FullMessage")]
        public string FullMessage { get; set; }
    }
}
