using System;
using System.IO;
using System.Net.Mime;
using System.Net.Http;
using System.Net.Http.Headers;
using ServiceStack;
using ServiceStack.Text;
using IAR.DispatcherAPI;
using IAR.DispatcherAPI.Controllers;

namespace IAR.DispatcherAPITest
{
    [TestClass]
    public class UnitTestAPI
    {

        [TestMethod]
        public void TestMethodByApi()
        {
            SendMessage();

        }
        [TestMethod]
        public void TestMethodByLocal()
        {
            DispatchMessageController.Post("");

        }

        private async void SendMessage()
        {
            Message message = new Message
            {
                ID = 0,
                ArrivedOn = DateTime.Now,
                MessageHeader = "X-Sender: luis.rosas@definityfirst.com|X-Receiver: 168e@iartest.com|MIME-Version: 1.0|From: luis.rosas@definityfirst.com|To: 168e@iartest.com; 159w@iartest.com|Date: 21 Jul 2016 13:22:12 -0500|Subject: Unable to process file|Content-Type: text/plain; charset=us-ascii|Content-Transfer-Encoding: quoted-printable|",
                MessageBody = "File FromXrec.eml on server DF-1211-01 could not be processed. Error: The 'unit1' start tag on line 7 position 6 does not match the end tag of 'units'. Line 10, position 5.\r\n",
                FullMessage = "Unable to process file\r\n File FromXrec.eml on server DF-1211-01 could not be processed. Error: The 'unit1' start tag on line 7 position 6 does not match the end tag of 'units'. Line 10, position 5.\r\n",
                DestinationEmailAddress = "168e@iartest.com; 159w@iartest.com",
                OriginationEmailAddress = "luis.rosas@definityfirst.com",
                SubscriberID = 0,
                Subject = "Unable to process file",
                XSender = "luis.rosas@definityfirst.com",
                XReceiver = "168e@iartest.com"
            };

            string messageAPI = "http://localhost:61146/";

            string url = messageAPI + "api/DispatchMessage/";

            string _apiKey = "4EB5F9184A482D155A392CD98B4A8C5D";

            using (HttpContent content = new StringContent(message.ToJson().SerializeToString()))
            using (var client = new HttpClient())
            {
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        "application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", _apiKey);

                try
                {
                    using (HttpResponseMessage response = await client.PostAsync(url, content))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"SendDispatchMessageMonitor: Could not send a dispatch message - request problem: {message.SubscriberID}, {message.Subject} - {ex} ");
                }
            }
        }
    }
}
