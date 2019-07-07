using System;

namespace Tools.Models
{
    public interface IRibbonButtonInfo : IComparable
    {
        string ID { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
        string Icon { get; set; }
        string Type { get; set; }
        string Assembly { get; set; }
    }
}
