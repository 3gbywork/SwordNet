using System;

namespace Excalibur.Models
{
    public class ContainerModel : IComparable
    {
        public ContainerModel(AppsApp app)
        {
            ID = app.ID;
            ProcessName = app.ProcessName;
            Param = app.Param;
            FullName = app.FullName.Replace(@"\\", @"/").Trim();
        }

        public string ID { get; set; }
        public string Alias { get; set; }
        public string ProcessName { get; set; }
        public string FullName { get; set; }
        public string Param { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is ContainerModel model)
            {
                return FullName.CompareTo(model.FullName);
            }
            return 0;
        }
    }
}
