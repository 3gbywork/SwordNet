using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Excalibur.Models;
using Utility.Config;

namespace Excalibur.Config
{
    class BreakpointConfigEntity : IXmlConfig
    {
        public delegate void ConfigChangedHandler(ObservableCollection<BreakpointModel> breaks);
        public event ConfigChangedHandler OnConfigChanged;

        public void Config(XmlElement xmlElement)
        {
            if (OnConfigChanged != null)
            {
                var breaks = GetConfigFromXml(xmlElement);
                OnConfigChanged.Invoke(breaks);
            }
        }

        private ObservableCollection<BreakpointModel> GetConfigFromXml(XmlElement xmlElement)
        {
            var result = new ObservableCollection<BreakpointModel>();
            var names = new HashSet<string>();

            string filename = string.Empty;

            var nodes = xmlElement.SelectNodes("Breakpoint");
            foreach (XmlElement node in nodes)
            {
                filename = node.GetAttribute("filename").Replace(@"//", @"/").Trim();
                if (!string.IsNullOrEmpty(filename) && names.Add(filename))
                {
                    BreakpointModel point = new BreakpointModel()
                    {
                        FileName = filename,
                        Description = node.GetAttribute("description"),
                        IsChecked = false,
                    };
                    result.Add(point);
                }
            }

            return result;
        }
    }
}
