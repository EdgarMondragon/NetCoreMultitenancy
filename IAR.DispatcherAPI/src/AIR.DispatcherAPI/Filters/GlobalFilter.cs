using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NLog;

namespace IAR.DispatcherAPI.Filters
{
    public class GlobalFilter : ActionFilterAttribute
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();


        public override void OnActionExecuting(ActionExecutingContext context)
        {
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
        }
      
    }
}
