using System;

namespace Tools.Models
{
    public class RibbonButtonBaseInfo : IRibbonButtonInfo
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public string Assembly { get; set; }

        public int CompareTo(object obj)
        {
            var info = obj as RibbonButtonBaseInfo;
            if (info == null)
            {
                return 0;
            }
            else
            {
                if (this.Assembly.Equals(info.Assembly))
                {
                    return this.Type.CompareTo(info.Type);
                }
                return this.Assembly.CompareTo(info.Assembly);
            }
        }
    }
}
