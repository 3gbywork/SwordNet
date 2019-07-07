using CommonUtility.Config;
using Excalibur.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Excalibur.Config
{
    class ContainerConfigEntity : IXmlConfig
    {
        public delegate void ConfigChangedHandler(ICollection<ContainerModel> containers);
        public event ConfigChangedHandler OnConfigChanged;

        public void Config(XmlElement xmlElement)
        {
            if (OnConfigChanged != null)
            {
                var containers = GetConfigFromXml(xmlElement);
                OnConfigChanged.Invoke(containers);
            }
        }

        private ICollection<ContainerModel> GetConfigFromXml(XmlElement xmlElement)
        {
            var result = new SortedSet<ContainerModel>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Apps));
            using (StringReader stringReader = new StringReader(xmlElement.OuterXml))
            {
                if (xmlSerializer.Deserialize(stringReader) is Apps apps
                    && null != apps.App)
                {
                    foreach (var app in apps.App)
                    {
                        result.Add(new ContainerModel(app));
                    }
                }
            }

            return result;
        }
    }
}
