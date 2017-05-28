using System.Collections;
using Tools.Models;

namespace Tools.Business
{
    interface IContentPanel
    {
        bool IsLoaded();
        IEnumerator GetEnumerator();
        object GetView(IRibbonButtonInfo info);
        object NewItem(IRibbonButtonInfo info, object view);
        void Add(object item);
    }
}
