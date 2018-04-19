using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Entities;
using AutoMapper;
using IAR.DispatcherAPI.Filters;
using IAR.DispatcherAPI.Interfaces;
using IAR.DispatcherAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace IAR.DispatcherAPI.Controllers
{
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    [Route("api/[controller]")]
    public class DispatchMessageController : Controller
    {

        private readonly IProcessor _processor;
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public static AppSettings _AppSettings { get; set; }
        public DispatchMessageController(IOptions<AppSettings> appSettings, IProcessor processor) : base()
        {
            _processor = processor;
            _AppSettings = appSettings.Value;
        }

        // GET: api/DispatchMessage
        [HttpGet]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/DispatchMessage/5
        [HttpGet("{id}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/DispatchMessage
        [HttpPost]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public async Task Post([FromBody]Message message)
        {
            try
            {
                DispatchMessage dispatchMessage = Mapper.Map<Message, DispatchMessage>(message);
                await Task.Run(() => _processor.ProcessMessage(dispatchMessage));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // PUT api/DispatchMessage/5
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/DispatchMessage/5
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public void Delete(int id)
        {
        }
    }
}
