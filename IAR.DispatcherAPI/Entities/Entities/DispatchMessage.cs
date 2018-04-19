using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations.Schema;


namespace Entities
{
    public class DispatchMessage : IEntityBase
    {
        public int Id { get; set; }
        public DateTime Arrivedon {get;set;}
        public string Messageheader { get; set; }
        public string Messagebody { get; set; }
        public int Subscriberid { get; set; }
        public string Destinationemailaddress { get; set; }
        public string Originationemailaddress { get; set; }
        public string Messagesubject { get; set; }
        [NotMapped]
        public string XSender { get; set; }
        [NotMapped]
        public string XReceiver { get; set; }

    }
}
