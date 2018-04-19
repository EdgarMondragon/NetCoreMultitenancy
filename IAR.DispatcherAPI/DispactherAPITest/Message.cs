using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispactherAPITest
{
    public class Message
    {
        public int ID { get; set; }
        public DateTime ArrivedOn { get; set; }
        public string MessageHeader { get; set; }
        public string MessageBody { get; set; }
        public int SubscriberID { get; set; }
        public string DestinationEmailAddress { get; set; }
        public string OriginationEmailAddress { get; set; }
        public string Subject { get; set; }
        //public int? Recentduplication { get; set; }
        //public int? Screenmsgduplicated { get; set; }
        //public int? Emailmsgduplicated { get; set; }
        //public int? Textmsgduplicated { get; set; }
        //public int? Dscreenmsgduplicated { get; set; }
        public string XSender { get; set; }
        public string XReceiver { get; set; }
        public string FullMessage { get; set; }
    }
}
