using System.ComponentModel;

namespace Excalibur.Models
{
    public class BreakpointModel : INotifyPropertyChanged
    {
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                DoNotification("IsChecked");
            }
        }
        public string FileName { get; set; }
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void DoNotification(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
