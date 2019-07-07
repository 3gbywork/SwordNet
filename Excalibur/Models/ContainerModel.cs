using System;

namespace Excalibur.Models
{
    public class ContainerModel : IComparable
    {
        public ContainerModel(AppsApp app)
        {
            //required
            ID = app.ID;
            ProcessName = app.ProcessName;
            FullName = app.FullName.Replace(@"\\", @"/").Trim();
            //optional
            Param = app.Param;
            WorkDir = app.WorkDir;
        }

        public string ID { get; set; }
        public string Alias { get; set; }
        public string ProcessName { get; set; }
        public string FullName { get; set; }
        public string Param { get; set; }
        public string WorkDir { get; set; }

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
