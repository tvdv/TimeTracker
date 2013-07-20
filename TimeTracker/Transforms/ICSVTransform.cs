using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Transforms
{
    interface ICSVTransform
    {
        string GetCSV(ICollectionView source);
    }
}
