using Excalibur.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Utility.Config;

namespace Excalibur.Config
{
    class BreakpointConfigEntity : IXmlConfig
    {
        public delegate void ConfigChangedHandler(ICollection<BreakpointModel> breaks);
        public event ConfigChangedHandler OnConfigChanged;

        public void Config(XmlElement xmlElement)
        {
            if (OnConfigChanged != null)
            {
                var breaks = GetConfigFromXml(xmlElement);
                OnConfigChanged.Invoke(breaks);
            }
        }

        private ICollection<BreakpointModel> GetConfigFromXml(XmlElement xmlElement)
        {
            var result = new SortedSet<BreakpointModel>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Breakpoints));
            using (StringReader stringReader = new StringReader(xmlElement.OuterXml))
            {
                if (xmlSerializer.Deserialize(stringReader) is Breakpoints breakpoint)
                {
                    if (breakpoint.Breakpoint != null)
                    {
                        foreach (var bpoint in breakpoint.Breakpoint)
                        {
                            result.Add(new BreakpointModel(bpoint));
                        }
                    }
                }
            }
            return result;
        }
    }
}
