using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Entities;
using IAR.DispatcherAPI.Infraestructure;
using IAR.DispatcherAPI.Interfaces;
using IAR.DispatcherAPI.Models;
using Microsoft.Extensions.Options;
using NLog;
using NuGet.Protocol.Core.v3;

namespace IAR.DispatcherAPI.Methods
{
    public class DispatchConnectorService : IDispatchConnectorService
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public static AppSettings _AppSettings { get; set; }

        public DispatchConnectorService(IOptions<AppSettings> appSettings)
        {
            _AppSettings = appSettings.Value;
        }

        public void SendMessage(DispatchMessage message, Task whenPosted)
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    await SendMessage(IncidentMessage.FromDispatchMessage(message));
                });

            }
            catch (Exception e)
            {
                Logger.Error("Could not send a dispatch message - request did not start: {0}, {1} - {2} ",
                                              message.Subscriberid, message.Messagesubject, e);
            }
        }

        private static async Task SendMessage(IncidentMessage message)
        {
            string apiUrl = _AppSettings.IarApiUrl ?? "https://localapi.iamresponding.com";

            string url = apiUrl + "/api/IncidentMessages/";

            Logger.Debug("Api service url: {0}", url);

            Ping pingSender = new Ping();

            PingReply reply = pingSender.Send(url);

            if (reply.Status == IPStatus.Success)
            {
                for (int i = 0; i < 5; i++)
                {
                    using (HttpContent content = new StringContent(message.ToJson()))
                    using (var _client = new HttpClient())
                    {
                        content.Headers.ContentType =
                            new MediaTypeHeaderValue("application/json");

                        _client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Token",
                                                          "unauthenticated");
                        _client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue(
                                "application/json"));

                        try
                        {
                            using (HttpResponseMessage response = await _client.PostAsync(url, content))
                            {
                                response.EnsureSuccessStatusCode();
                                Logger.Info("SendMessage: Sent message to IncidentMessages API - {0}", message);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("SendMessage: Could not send a dispatch message - request problem: {0}, {1} - {2} ",
                                             message.SubscriberId, message.Subject, e);
                            i++;
                        }
                    }
                }
            }

        }
    }
}
