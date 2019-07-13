using ChatBeet.Queuing.Rules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ChatBeet.Queuing
{
    public class QueueConfigurationAccessor
    {
        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };
        private string FileName = "rules.json";
        private List<Rule> Rules = new List<Rule>();

        public QueueConfigurationAccessor()
        {
            Task.Run(() => Load());
        }

        private async Task Load()
        {
            using (var file = File.OpenText(FileName))
            using (var jtr = new JsonTextReader(file))
            {
                var js = new JsonSerializer();
                js.TypeNameHandling = TypeNameHandling.Auto;
                Rules = js.Deserialize<List<Rule>>(jtr);
            }
        }

        private async Task Save()
        {
            using (var file = File.Create(FileName))
            using (var fileWriter = new StreamWriter(file))
            using (var writer = new JsonTextWriter(fileWriter))
            {
                var js = new JsonSerializer();
                js.TypeNameHandling = TypeNameHandling.Auto;
                js.Serialize(writer, Rules);
            }
        }
    }
}
