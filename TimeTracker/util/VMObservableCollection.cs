using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TimeTracker.util
{
    /// <summary>
    /// An ObservableCollection of ViewModels that syncs to an underlying ObservableCollection of models.
    /// No Thread Safety.
    /// </summary>
    /// <typeparam name="T">ViewModel Type</typeparam>
    /// <typeparam name="MT">Model Type</typeparam>
    public class VmObservableCollection<T,MT> : ObservableCollection<T> 
    {
        private readonly ObservableCollection<MT> _modelCollection;
        private readonly Func<MT,T> _createFn;
        private readonly Func<MT, T,bool> _findFn;

        public VmObservableCollection(ObservableCollection<MT> modelCollection,Func<MT,T> createFn ,Func<MT,T,bool> findFn  )
        {
            _modelCollection = modelCollection;
            _createFn = createFn;
            _findFn = findFn;
            _modelCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_modelCollection_CollectionChanged);
        }

        void _modelCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    this.Add(_createFn((MT)item));
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    //TODO optimise
                    var vm=this.FirstOrDefault(z=>_findFn((MT) item,z));
                    this.Remove(vm);
                }
            }
        }
    }
}
