using System;

namespace Excalibur.Models
{
    public class BreakpointModel : ModelBase, IComparable
    {
        private bool isChecked;
        private BreakpointsBreakpoint breakpoint;

        public BreakpointModel(BreakpointsBreakpoint breakpoint)
        {
            this.breakpoint = breakpoint;
            Description = breakpoint.Description;
            FileName = breakpoint.Filename;
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                DoNotification(nameof(IsChecked));
            }
        }
        public string FileName { get; set; }
        public string Description { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is BreakpointModel model)
            {
                return FileName.CompareTo(model.FileName);
            }
            return 0;
        }
    }
}
