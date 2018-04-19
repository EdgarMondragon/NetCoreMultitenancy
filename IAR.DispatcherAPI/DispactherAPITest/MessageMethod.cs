using Entities;
using IAR.DispatcherAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispactherAPITest
{
    public class MessageMethod
    {
        private readonly IProcessor _processor;
        public MessageMethod(IProcessor processor) : base()
        {
           _processor = processor;
        }

        public void SendMessage()
        {
            DispatchMessage dispatchMessage = new DispatchMessage
            {
                Id = 0,
                Arrivedon = DateTime.Now,
                Messageheader = "X-Sender: luis.rosas@definityfirst.com|X-Receiver: 168e@iartest.com|MIME-Version: 1.0|From: luis.rosas@definityfirst.com|To: 168e@iartest.com; 159w@iartest.com|Date: 21 Jul 2016 13:22:12 -0500|Subject: Unable to process file|Content-Type: text/plain; charset=us-ascii|Content-Transfer-Encoding: quoted-printable|",
                Messagebody = "File FromXrec.eml on server DF-1211-01 could not be processed. Error: The 'unit1' start tag on line 7 position 6 does not match the end tag of 'units'. Line 10, position 5.\r\n",
                Destinationemailaddress = "168e@iartest.com; 159w@iartest.com",
                Originationemailaddress = "luis.rosas@definityfirst.com",
                Subscriberid = 0,
                Messagesubject = "Unable to process file",
                XSender = "luis.rosas@definityfirst.com",
                XReceiver = "168e@iartest.com"
            };

            _processor.ProcessMessage(dispatchMessage);
        }
    }

}
