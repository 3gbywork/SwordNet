using CommonUtility.Config;
using Excalibur.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Excalibur.Config
{
    class ConsoleConfigEntity : IXmlConfig
    {
        public delegate void ConfigChangedHandler(ICollection<ConsoleModel> consoles);
        public event ConfigChangedHandler OnConfigChanged;

        public void Config(XmlElement xmlElement)
        {
            if (OnConfigChanged != null)
            {
                var consoles = GetConfigFromXml(xmlElement);
                OnConfigChanged.Invoke(consoles);
            }
        }

        private ICollection<ConsoleModel> GetConfigFromXml(XmlElement xmlElement)
        {
            var result = new SortedSet<ConsoleModel>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Agents));
            using (StringReader stringReader = new StringReader(xmlElement.OuterXml))
            {
                if (xmlSerializer.Deserialize(stringReader) is Agents agents)
                {
                    if (agents.Agent != null)
                    {
                        foreach (var agent in agents.Agent)
                        {
                            result.Add(new ConsoleModel(agent));
                        }
                    }
                }
            }

            return result;
        }
    }
}
