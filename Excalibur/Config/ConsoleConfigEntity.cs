using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Excalibur.Models;
using Utility.Config;

namespace Excalibur.Config
{
    class ConsoleConfigEntity : IXmlConfig
    {
        public delegate void ConfigChangedHandler(ObservableCollection<ConsoleModel> consoles);
        public event ConfigChangedHandler OnConfigChanged;

        public void Config(XmlElement xmlElement)
        {
            if (OnConfigChanged != null)
            {
                var consoles = GetConfigFromXml(xmlElement);
                OnConfigChanged.Invoke(consoles);
            }
        }

        private ObservableCollection<ConsoleModel> GetConfigFromXml(XmlElement xmlElement)
        {
            var result = new ObservableCollection<ConsoleModel>();
            var paths = new HashSet<string>();

            string path = string.Empty;

            var nodes = xmlElement.SelectNodes("Agent");
            foreach (XmlElement node in nodes)
            {
                path = node.GetAttribute("path").Replace(@"//", @"/");
                if (!string.IsNullOrEmpty(path) && paths.Add(path))
                {
                    ConsoleModel consoleModel = new ConsoleModel
                    {
                        Path = path,
                        Alias = node.GetAttribute("alias"),
                        Name = node.GetAttribute("name"),
                        Param = node.GetAttribute("param")
                    };
                    result.Add(consoleModel);
                }
            }

            return result;
        }
    }
}
