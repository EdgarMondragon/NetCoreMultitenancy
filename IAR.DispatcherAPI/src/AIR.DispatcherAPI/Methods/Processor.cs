using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using DataAccess.Abstract;
using IAR.DispatcherAPI.Interfaces;
using NLog;
using System.Net.Mail;
using System.Net;
using IAR.DispatcherAPI.Model;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using IAR.DispatcherAPI.Infraestructure;
using IAR.DispatcherAPI.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace IAR.DispatcherAPI
{

    public class Processor : IProcessor
    {

        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        private static IDataAccessProcessor _dataAccessProcessor;
        private static IDispatchConnectorService _dispatchConnectorService;
        private static IDispatchUnmatchedMessagesRepository _dispatchUnmatchedMessagesRepository;
        public static AppSettings _AppSettings { get; set; }
        public static MailSettings _MailSettings { get; set; }

        public Processor(IOptions<AppSettings> appSettings, IOptions<MailSettings> mailSettings,
            IDispatchUnmatchedMessagesRepository dispatchUnmatchedMessagesRepository, IDataAccessProcessor dataAccessProcessor, IDispatchConnectorService dispatchConnectorService)
        {
            
            _AppSettings = appSettings.Value;
            _MailSettings = mailSettings.Value;
            _dispatchConnectorService = dispatchConnectorService;
            _dataAccessProcessor = dataAccessProcessor;
            _dispatchUnmatchedMessagesRepository = dispatchUnmatchedMessagesRepository;
        }

        #region Implementation
        public async Task ProcessMessage(DispatchMessage dispatchMessage)
        {
            try
            {
                Logger.Info("Removing Invalid Characters in Message");

                dispatchMessage.Messagebody = RemoveInvalidChars(dispatchMessage.Messagebody);
                if (InvalidMessage(dispatchMessage)) return;

                Logger.Debug("Will remove comments from origination: <{0}>", dispatchMessage.Originationemailaddress);
                dispatchMessage.Originationemailaddress = RemoveComments(dispatchMessage.Originationemailaddress);
                Logger.Debug("Comments removed from origination: <{0}>", dispatchMessage.Originationemailaddress);

                await Task.Run(() => SendMessages(dispatchMessage));
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.ToString());
            }
        }

        #endregion

        #region Private Methods

        private string RemoveInvalidChars(string messagebody)
        {
            char[] invalidcahrs = { '', 'ï', '»', '¿' };

            foreach (char c in invalidcahrs)
            {
                messagebody = messagebody.Replace("" + c, "");
            }
            Logger.Info("Modified Message " + messagebody);
            return messagebody;
        }

        private bool InvalidMessage(DispatchMessage message)
        {

            string msgToReplace = "";
            string msgToReplaceOnlyWithValidDomain = "";

            //bool emptyFlag = false;
            for (int i = 0; i < message.Destinationemailaddress.Split(',').Count(); i++)
            {
                if (i == message.Destinationemailaddress.Split(',').Count() - 1)
                {
                    msgToReplace += message.Destinationemailaddress.Split(',')[i];
                    if (message.Destinationemailaddress.Split(',')[i].ToLower().Contains(_AppSettings.Domain))
                    {
                        msgToReplaceOnlyWithValidDomain += message.Destinationemailaddress.Split(',')[i];
                    }
                }
                else
                {
                    msgToReplace += message.Destinationemailaddress.Split(',')[i] + ",";
                    if (message.Destinationemailaddress.Split(',')[i].ToLower().Contains(_AppSettings.Domain))
                    {
                        msgToReplaceOnlyWithValidDomain += message.Destinationemailaddress.Split(',')[i] + ",";
                    }
                }
            }

            // Validating all recipient fields; from custom parser and mailbee.
            Logger.Debug("Comparing receiver:{0}, xreceiver:{1}, mailbee recipient:{2} with {3}",
                message.Destinationemailaddress,
                message.XReceiver,
                msgToReplace.ToLower(),
                _AppSettings.Domain);


            string destinationAddr = message.Destinationemailaddress ?? "";

            if (!(!string.IsNullOrEmpty(msgToReplaceOnlyWithValidDomain)
                  || CheckDestinationAddress(destinationAddr, _AppSettings.Domain)
                  || message.XReceiver.ToLower().Contains(_AppSettings.Domain)))
            {
                //Should not be processed because it's not within domain
                Logger.Debug("File won't be processed. Domain: {0}, Mailbee: {1}, recipient:{2}, xreceiver:{3}",
                    _AppSettings.Domain,
                    msgToReplace,
                    destinationAddr,
                    message.XReceiver);

                ConfirmUnprocessedMessage(string.Format(
                    "File won't be processed. Domain: {0}, Mailbee: {1}, recipient:{2}, xreceiver:{3}. Server: {4}",
                    _AppSettings.Domain,
                    msgToReplace,
                    destinationAddr,
                    message.XReceiver,
                    Environment.MachineName),
                    true);
                return true;
            }

            message.Destinationemailaddress = msgToReplaceOnlyWithValidDomain;
            Logger.Debug("Body read");

            if (string.IsNullOrEmpty(message.Originationemailaddress))
            {
                // There is no from
                Logger.Error("Origination [{0}] cannot be parsed. Headers: {1}",
                    message.Originationemailaddress,
                    message.Messageheader);

                var errorMessageBody = string.Format(
                    "Message in {0} could not be processed due to missing origination.",
                    Environment.MachineName);
                ConfirmUnprocessedMessage(errorMessageBody, false);

                return true;
            }

            if (string.IsNullOrEmpty(message.Destinationemailaddress) && string.IsNullOrEmpty(message.XReceiver))
            {
                // There is from but not to nor x-receiver
                Logger.Error("Destination [{0}] and x-receiver [{1}] cannot be parsed. Headers: {2}",
                    message.Destinationemailaddress,
                    message.XReceiver,
                    message.Messageheader);

                var errorMessageBody = string.Format(
                    "Message in {0} could not be processed due to missing  destination.", Environment.MachineName);
                ConfirmUnprocessedMessage(errorMessageBody, false);

                return true;
            }

            return false;
        }
        private bool CheckDestinationAddress(string destinationAddr, string domain)
        {
            return destinationAddr != null && destinationAddr.ToLower().Contains(domain);
        }
        private void ConfirmUnprocessedMessage(string errorMessageBody, bool sendEmailNotification)
        {
            string from = _AppSettings.EmlOnErrorEmailFrom;
            string to = _AppSettings.EmlOnErrorEmailTo;
            string moveUnprocessedMessage = _AppSettings.SaveUnprocessedMessage;
            string notifyEmlErrors = _AppSettings.NofityEmlErrors;

            Logger.Info(
                "Confirm unprocessed file. Parameters, from:<{0}>, to:<{1}>, message unprocessed:<{2}>, notify errors:<{3}>",
                from,
                to,
                moveUnprocessedMessage,
                notifyEmlErrors);

            try
            {
                if (notifyEmlErrors != "true" || !sendEmailNotification) return;
                Logger.Debug("Sending email error: {0}", errorMessageBody);

                SendMail(@from, to, "Unable to process message", errorMessageBody);
            }
            catch (System.Exception smtpEx)
            {
                Logger.Error("Could not send email. SMTP error: {0}", smtpEx.Message);
            }
        }

        private void SendMail(string from, string to, string subject, string body)
        {
            body = WebUtility.HtmlDecode(body);
            Logger.Info("Sending email message from:{0}, to:{1}, subject:{2}, body:{3}",
                from,
                to,
                subject,
                body);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("",from));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_MailSettings.SMTPName, UtilsHelper.UtilsHelper.ParseInt(_MailSettings.SMTPPort, 0), false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                
                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate(_AppSettings.SMTPUser, _AppSettings.SMTPPassword);

                client.Send(message);
                client.Disconnect(true);
            }

        }

        private string RemoveComments(string line)
        {
            // Repeat until no parenthesis found
            while (line.IndexOf('(') != -1)
            {
                // If there are parethesis
                // Get the position of open parenthesis
                int open = line.IndexOf('(');

                // Look for the close parenthesis starting from open
                int close = line.IndexOf(')', open);

                // Remove everything between parenthesis inclusive
                if (close != -1 && close > open)
                {
                    string replaceThis = line.Substring(open, (close - open) + 1);
                    line = line.Replace(replaceThis, string.Empty);
                }
            }
            return line.Trim();
        }

        private async Task SendMessages(DispatchMessage message)
        {
            List<SubscriberByOriginationDestination> ds = new List<SubscriberByOriginationDestination>();
            int matchedSubscribers = 0;

            matchedSubscribers = await LookForMatchedSubscribers(message, ds);

            var matchingSubscriberIds = from a in ds.AsEnumerable()
                                        select a.EmailAddress;

            Logger.Debug("All matching addresses after check: {0}", string.Join(",", matchingSubscriberIds.ToList()));

            if (matchedSubscribers == 0)
            {
                Logger.Info("No subscriber matched origination <{0}>, destination <{1}>, xreceiver:<{2}>",
                    message.Originationemailaddress, message.Destinationemailaddress, message.XReceiver);

               await Task.Run(() =>  SaveUnmatchedDispatch(message));

                var errorMessageBody =
                    $"Messages on server {Environment.MachineName} could not be matched to a subscriber. <{message.Originationemailaddress}><{message.Destinationemailaddress}>|<{message.XSender}><{message.XReceiver}>.";
                await Task.Run(() => ConfirmUnprocessedMessage(errorMessageBody, false));
            }

            Logger.Debug("Will send messages to each confirmed subscriber");

            await Task.Run(() => SendEmailToConfirmedSubscribers(ds, message));
        }

        private async Task SendEmailToConfirmedSubscribers(List<SubscriberByOriginationDestination> ds, DispatchMessage message)
        {
            foreach (var item in ds)
            {
                if (string.IsNullOrEmpty(item.Id.ToString())) continue;

                message.Subscriberid = UtilsHelper.UtilsHelper.ParseInt(item.Id, 0);

                Logger.Debug("Parsed subscriber id is: {0}", message.Subscriberid);

                if (message.Subscriberid == 0) return;

                var subsRow = _dataAccessProcessor.GetSubscriptor(message.Subscriberid);

                bool UseAttachment = UtilsHelper.UtilsHelper.ParseBool(subsRow.Usedispatchfromattachment, false);

                Logger.Debug("Origination and destination matched to subscriber: {0}",
                    message.Subscriberid);

                if (UseAttachment)
                {
                    Logger.Debug("Getting dispatch from attachments");

                    message.Messagebody = AttachmentManager.GetMessage(new MailMessage(), _AppSettings.AttachmentsPath,
                        _AppSettings.ValidAttachmentTypes);
                }

                // fix for old iOS apps that included data detectors
                if (message.Messagebody.Contains("<x-apple-data-detectors"))
                {
                    int iStart = message.Messagebody.ToLower().IndexOf("<x-apple-data-detectors", StringComparison.Ordinal);
                    int iEnd = message.Messagebody.ToLower().IndexOf(">", iStart, StringComparison.Ordinal);
                    string appleDataDetector = message.Messagebody.Substring(iStart, iEnd - iStart + 1);
                    message.Messagebody = message.Messagebody.Replace(appleDataDetector, "");
                }

                // Encode HTML - This prevents cross-site scripting
                message.Messagebody = WebUtility.HtmlDecode(message.Messagebody);
                Logger.Debug("Body encoded");

                message.Destinationemailaddress = UtilsHelper.UtilsHelper.ParseString(item.EmailAddress);

                if (await ShouldIgnoreDuplicatedMessage(message))
                {
                    continue;
                }

                message = _dataAccessProcessor.SaveMessage(message);

                Logger.Info("Message record saved for subscriber: {0}, message id: {1}",
                            message.Subscriberid,
                            message.Id);

                _dispatchConnectorService.SendMessage(message, Task.FromResult(0));
            }
        }

        private async Task<bool> ShouldIgnoreDuplicatedMessage(DispatchMessage message)
        {
            DispatchMessage lastDispatchMessage = await _dataAccessProcessor.GetLatestDispatchMessageForSubscriber(message.Subscriberid);
            if (lastDispatchMessage == null)
            {
                Logger.Debug("Did not find any last dispatch message");
                return false;
            }

            if (IsSameBody(message, lastDispatchMessage)
               && IsRecentDate(lastDispatchMessage)
               && IsRecentDestination(message, lastDispatchMessage))
            {
                Logger.Debug("Ignoring duplicated message");
                return true;
            }

            return false;
        }

        private static bool IsRecentDestination(DispatchMessage message, DispatchMessage lastDispatchMessage)
        {
            return lastDispatchMessage.Destinationemailaddress.Contains(message.Destinationemailaddress);
        }

        private async Task<int> LookForMatchedSubscribers(DispatchMessage message, List<SubscriberByOriginationDestination> ds)
        {
            Logger.Debug("Checking all originations vs all destinations");
            int retries = 1;
            int matchingAttempts = 1;
            int matchedSubscribers = 0;
            ds = default(List<SubscriberByOriginationDestination>);
            while (retries <= 5)
            {
                try
                {
                    while (matchedSubscribers == 0 && matchingAttempts <= 3)
                    {
                        ds = message.XReceiver.ToLower().Contains(_AppSettings.Domain)
                            ? await  _dataAccessProcessor.GetSubscriberByOriginationDestination(
                                string.Join(",", (object)message.Originationemailaddress, message.XSender),
                                string.Join(",", (object)message.XReceiver, message.Destinationemailaddress))
                            : await _dataAccessProcessor.GetSubscriberByOriginationDestination(
                                string.Join(",", (object)message.Originationemailaddress, message.XSender),
                                message.Destinationemailaddress);

                        matchedSubscribers = (ds == null) ? 0 : UtilsHelper.UtilsHelper.ParseInt(ds.Count, 0);

                        //if (matchedSubscribers == 0 && matchingAttempts < 3) //Thread.Sleep(5000);
                        matchingAttempts++;
                    }
                    Logger.Info("Number of matched subscribers for Origination <{0}> and destination <{1}> are {2}",
                        string.Join(",", (object)message.Originationemailaddress, message.XSender),
                        string.Join(",", (object)message.Destinationemailaddress, message.XReceiver),
                        matchedSubscribers);
                    return matchedSubscribers;
                }
                catch (Exception exception)
                {
                    Logger.Error("EXCEPTION: SP: GetSubscriberByOriginationDestination. RETRY #{0}", retries, exception);

                    if ((!exception.ToString().Contains("deadlock") &&
                         !exception.ToString().Contains("query processor could not start")) || retries == 5)
                        throw;
                    //Thread.Sleep(5000);
                    retries++;
                }
            }
            return matchedSubscribers;
        }

        private async Task SaveUnmatchedDispatch(DispatchMessage message)
        {
            DispatchUnmatchedMessages ds = new DispatchUnmatchedMessages();
            int IDvalue = 0;

          
            ds.Arrivedon = DateTime.Now;
            ds.Messageheader = message.Messageheader;
            ds.Messagesubject = message.Messagesubject;
            ds.Messagebody = message.Messagebody;
            ds.Messagefrom = message.Originationemailaddress;
            ds.Messageto = message.Destinationemailaddress;

            char[] letters = "@abcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] numbers = "0123456789".ToCharArray();
            int letterPosition = -1;
            int numberPosition = -1;
            int atPosition = -1;
            string newDestinationsIDs = string.Empty;
            string destinationEmailAddress = message.Destinationemailaddress;
            if (message.XReceiver.ToLower().Contains(_AppSettings.Domain))
                destinationEmailAddress = string.Join(",", (object) message.Destinationemailaddress,
                    message.XReceiver.ToLower());

            string[] destinationIDs = destinationEmailAddress.ToLower().Split(',');

            foreach (string s in destinationIDs)
            {
                letterPosition = s.IndexOfAny(letters);
                Logger.Debug("string: {0}, letterPosition: {1}", s, letterPosition);
                if (letterPosition > 0)
                    newDestinationsIDs = string.Join(",", newDestinationsIDs, s.Substring(0, letterPosition));
                else
                {
                    numberPosition = s.IndexOfAny(numbers);
                    if ((letterPosition == 0) && (numberPosition > 0))
                    {
                        string newS = s.Substring(numberPosition);
                        atPosition = newS.IndexOfAny(letters);
                        newDestinationsIDs = string.Join(",", newDestinationsIDs, newS.Substring(0, atPosition));
                        Logger.Debug("string: {0}, letterPosition: {1}, numberPosition: {2}, newDestinationsIDs: {3}",
                            s, letterPosition, numberPosition, newDestinationsIDs);
                    }
                }
            }

            if (newDestinationsIDs.IndexOf(",", StringComparison.Ordinal) == 0)
                newDestinationsIDs = newDestinationsIDs.Substring(1);
            Logger.Debug("newDestinationsIDs, suspected subscribers: {0}", newDestinationsIDs);

            // clean up subscriberIDs
            newDestinationsIDs = Clean(newDestinationsIDs);

            List<SubscriberInfoBySubscriberIds> SubscriberDS =
               await _dataAccessProcessor.GetSubscriberInfoBySubscriberIdsInUnmatchedDispatch(newDestinationsIDs);
            bool sendUnmatchedDispatchMessage = false;
            if (SubscriberDS != null && SubscriberDS.Count > 0)
            {
                SubscriberInfoBySubscriberIds subscriberInfo = SubscriberDS.FirstOrDefault();

                ds.Destsubscriberid = subscriberInfo.Id;
                ds.Destsubscribername = subscriberInfo.Subscribername;
                ds.Destsubscriberstatusid = subscriberInfo.Statusid;
                ds.Destsubscriberstatusname = subscriberInfo.Statusname;

                sendUnmatchedDispatchMessage = (subscriberInfo.Statusid == 1 || subscriberInfo.Statusid == 7);

                Logger.Debug("Unmatcheddispatch subscriber information. FullName: {0}, ScreenName: {1}, AgencyLogin: {2}, Status: {3}, County: {4}, State: {5}, From Header: {6}",
                   subscriberInfo.Fullname,
                   subscriberInfo.Subscribername,
                   subscriberInfo.Loginname,
                   subscriberInfo.Statusname,
                   subscriberInfo.Mailingcountry,
                   subscriberInfo.Mailingstate,
                   string.Join(",", message.XSender, message.Originationemailaddress));

                Logger.Debug("Send mail with unmatched dispatch subscriber information: {0}", sendUnmatchedDispatchMessage);

                _dispatchUnmatchedMessagesRepository.AddAsync(ds);
                
                Logger.Info("Unmatched dispatch subscriber stored in the database");

                if (sendUnmatchedDispatchMessage)
                {
                    SendUnmatchedDispatchMail(subscriberInfo,
                        string.Join(",", message.XSender, message.Originationemailaddress),
                        message.Messagebody, string.Join(",", message.XReceiver, message.Destinationemailaddress));
                }
            }
        }

        private void SendUnmatchedDispatchMail(SubscriberInfoBySubscriberIds ds, string dispatchFromData, string messagebody, string toData)
        {
            string from = _AppSettings.EmlOnErrorEmailFrom;
            string to = _AppSettings.EmlOnUnmmatchedDispatchEmailTo;

            string subject = string.Format("Unmatched dispatch for [{0}]", ds.Fullname);

            string messageBody = string.Format("An unmatched dispatch has been received for this subscriber.");
            messageBody = string.Join(Environment.NewLine, messageBody, string.Format("Their on-screen name is: [{0}]", ds.Subscribername));
            messageBody = string.Join(Environment.NewLine, messageBody, string.Format("Their agency log-in name is: [{0}]", ds.Loginname));
            messageBody = string.Join(Environment.NewLine, messageBody, string.Format("Their current status is: [{0}]", ds.Statusname));
            messageBody = string.Join(Environment.NewLine, messageBody, string.Format("The dispatch came from: [{0}]", dispatchFromData));
            messageBody = string.Join(Environment.NewLine, messageBody, string.Format("They are located in: [{0}], [{1}]", ds.Mailingcountry, ds.Mailingstate));
            messageBody = string.Join(Environment.NewLine, messageBody, string.Format("The message was: [{0}]", messagebody));
            messageBody = string.Join(Environment.NewLine, messageBody, string.Format("This message was sent to: [{0}]", toData));

            try
            {
                SendMail(from, to, subject, messageBody);
            }
            catch (Exception smtpEx)
            {
                Logger.Error("Could not send unmatcheddispatch info email. Error: {0}", smtpEx.Message);
            }
        }

        private string Clean(string IDs)
        {
            var sb = new StringBuilder(IDs);
            // Reserved Characters
            // RFC 3986
            // gen-delims 
            sb.Replace(":", "");
            sb.Replace("/", "");
            sb.Replace("?", "");
            sb.Replace("#", "");
            sb.Replace("[", "");
            sb.Replace("]", "");
            sb.Replace("@", "");
            // sub-delims 
            sb.Replace("!", "");
            sb.Replace("$", "");
            sb.Replace("&", "");
            sb.Replace("'", "");
            sb.Replace("(", "");
            sb.Replace(")", "");
            sb.Replace("*", "");
            sb.Replace("+", "");
            sb.Replace(";", "");
            sb.Replace("=", "");
            return sb.ToString();
        }

        private static bool IsSameBody(DispatchMessage message, DispatchMessage lastDispatchMessage)
        {
            return message.Messagebody == lastDispatchMessage.Messagebody;
        }

        private static bool IsRecentDate(DispatchMessage lastDispatchMessage)
        {
            DateTime convertedDate = UtilsHelper.UtilsHelper.ParseDateTime(lastDispatchMessage.Arrivedon, new DateTime(1900, 1, 1));

            int MaxTimeSpan = UtilsHelper.UtilsHelper.ParseInt(_AppSettings.MaxMessageSpan, 1);

            return (DateTime.UtcNow - convertedDate).TotalMinutes <= MaxTimeSpan;
        }


        #endregion

    }
}
