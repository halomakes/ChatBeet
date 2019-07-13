using ChatBeet.Queuing.Rules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ChatBeet.Queuing
{
    public class QueueConfigurationAccessor
    {
        private string FileName = "rules.json";
        private List<Rule> Rules = new List<Rule>();

        public QueueConfigurationAccessor()
        {
            Load();
        }

        public List<Rule> GetRules() => Rules;

        private void Load()
        {
            using (var file = File.OpenText(FileName))
            using (var jtr = new JsonTextReader(file))
            {
                var js = new JsonSerializer();
                js.TypeNameHandling = TypeNameHandling.Auto;
                Rules = js.Deserialize<List<Rule>>(jtr);
            }
        }

        private void Save()
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
