using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DataAccess.Abstract;
using IAR.DispatcherAPI.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Mysqlx.Notice;
using NLog;
using NLog.Fluent;
using NuGet.Protocol.Core.v3;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Tls;
using LogLevel = NLog.LogLevel;
using MD5 = MimeKit.Cryptography.MD5;


namespace IAR.DispatcherAPI.Filters
{
    public class ApiAuthorizationFilter : ActionFilterAttribute
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        private static IDispatchCentersRepository _dispatchCentersRepository;

        public ApiAuthorizationFilter(IDispatchCentersRepository dispatchCentersRepository)
        {
            _dispatchCentersRepository = dispatchCentersRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = AuthenticateRequest(context.HttpContext.Request);

            if (user != null)
            {
                return;
            }
            context.Result = new BadRequestResult();
        }

        private static User AuthenticateRequest(HttpRequest request)
        {
            var apiRequest = request.Headers["Authorization"];
            User usr = null;
            if (string.IsNullOrEmpty(apiRequest))
            {
                Logger.Log(LogLevel.Warn, "Request can't be authenticated - no authorization header or apiKey parameter");
                return null;
            }
            else
            {
                try
                {

                    apiRequest = apiRequest.ToString().Replace("ApiKey", "").Trim();
                    var apitext = _dispatchCentersRepository.GetSingle(d => d.ApiKey == apiRequest);
                    usr = new User { ApiKey = apitext.ApiKey, Name = apitext.DispatcherMasterName, Password = apitext.DispatcherUserPassword };
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Warn, "Request can't be authenticated - no authorization header or apiKey parameter");
                    return null;
                }
            }
            return usr;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Logger.Log(LogLevel.Warn, "OnActionExecuting");

        }
    }
}
