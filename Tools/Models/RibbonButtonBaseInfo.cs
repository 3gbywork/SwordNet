using System;
using System.Xml.Serialization;

namespace Tools.Models
{
    [Serializable]
    public abstract class RibbonButtonBaseInfo : IRibbonButtonInfo
    {
        [XmlAttribute] public string ID { get; set; }
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string DisplayName { get; set; }
        [XmlAttribute] public string Icon { get; set; }
        [XmlAttribute] public string Type { get; set; }
        [XmlAttribute] public string Assembly { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is IRibbonButtonInfo info)
            {
                //if (Assembly.Equals(info.Assembly))
                //{
                //    return Type.CompareTo(info.Type);
                //}
                //return Assembly.CompareTo(info.Assembly);
                return ID.CompareTo(info.ID);
            }
            return 0;
        }
    }
}
