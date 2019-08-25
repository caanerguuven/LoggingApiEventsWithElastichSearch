using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LoggingApiExample.Controllers
{
    [Route("MyApi/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IConfiguration configuration;
        private ILogger logger;

        public ValuesController(IConfiguration _configuration,ILoggerFactory _loggerFactory) {
            configuration = _configuration;
            logger = _loggerFactory.CreateLogger("MyApi Logger");
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            logger.LogInformation("My Api was called by anyone - Hello World .This is Get Method");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            logger.LogInformation("My Api was called by anyone - Hello World .This is Get Method With Id");
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            try
            {
                logger.LogInformation("My Api was called by anyone - Hello World .This is Post Method");
            }
            catch (Exception ex)
            {
                logger.LogError("Ouch ! An error was occured. I must inform.");
                throw ex;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            logger.LogInformation("My Api was called by anyone - Hello World .This is Put Method");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            logger.LogInformation("My Api was called by anyone - Hello World .This is Delete Method. I suggest you to be careful.");
        }
    }
}
