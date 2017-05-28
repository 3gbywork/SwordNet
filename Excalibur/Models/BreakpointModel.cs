using System.ComponentModel;

namespace Excalibur.Models
{
    public class BreakpointModel : ModelBase
    {
        private bool isChecked;
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
    }
}
