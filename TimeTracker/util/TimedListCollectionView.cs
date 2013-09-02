using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TimeTracker.util
{
    class TimedListCollectionView : ListCollectionView
    {
        public TimedListCollectionView(IList list) : base(list)
        {
        }
        protected override void RefreshOverride()
        {
            System.Diagnostics.Debug.WriteLine("TimedListCollectionView. Refresh start " + DateTime.Now.ToLongTimeString());
            base.RefreshOverride();
            System.Diagnostics.Debug.WriteLine("TimedListCollectionView. Refresh end " + DateTime.Now.ToLongTimeString());
        }
    }
}
