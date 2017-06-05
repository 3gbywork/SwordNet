using System;
using System.Xml.Serialization;

namespace Tools.Models
{
    [Serializable]
    public abstract class RibbonButtonBaseInfo : IRibbonButtonInfo
    {
        [XmlAttribute]
        public virtual string Name { get; set; }
        [XmlAttribute]
        public virtual string Icon { get; set; }
        [XmlAttribute]
        public virtual string Type { get; set; }
        [XmlAttribute]
        public virtual string Assembly { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is IRibbonButtonInfo info)
            {
                if (Assembly.Equals(info.Assembly))
                {
                    return Type.CompareTo(info.Type);
                }
                return Assembly.CompareTo(info.Assembly);
            }
            return 0;
        }
    }
}
