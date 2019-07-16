using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChatBeet.Queuing;
using ChatBeet.Queuing.Rules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatBeet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsoleController : ControllerBase
    {
        private readonly IMessageQueueService queueService;
        private readonly QueueConfigurationAccessor configurationAccessor;

        public ConsoleController(IMessageQueueService queueService, QueueConfigurationAccessor configurationAccessor)
        {
            this.queueService = queueService;
            this.configurationAccessor = configurationAccessor;
        }

        [HttpPost("send")]
        public ActionResult SendMessage([FromForm]OutputMessage msg)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            queueService.PushRaw(msg);
            return Ok();
        }

        [HttpGet("rules")]
        public ActionResult<IEnumerable<Rule>> GetRules() => Ok(configurationAccessor.GetRules());

        [HttpPatch("rules")]
        public ActionResult ReloadRules()
        {
            configurationAccessor.Load();
            return Ok();
        }

        [HttpPut("rules")]
        public ActionResult SetRules(IFormFile file)
        {
            var rules = new List<Rule>();
            try
            {
                using (var stream = file.OpenReadStream())
                using (var text = new StreamReader(stream))
                using (var jtr = new JsonTextReader(text))
                {
                    var js = new JsonSerializer();
                    js.TypeNameHandling = TypeNameHandling.Auto;
                    rules = js.Deserialize<List<Rule>>(jtr);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            configurationAccessor.Set(rules);
            return Ok(rules);
        }
    }
}