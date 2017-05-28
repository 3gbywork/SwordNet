using System;

namespace Tools.Models
{
    public interface IRibbonButtonInfo : IComparable
    {
        string Name { get; set; }
        string Icon { get; set; }
        string Type { get; set; }
        string Assembly { get; set; }
    }
}
