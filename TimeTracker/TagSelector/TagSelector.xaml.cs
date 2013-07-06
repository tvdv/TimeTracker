using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeTracker.Annotations;
using TimeTracker.Model;

namespace TimeTracker.TagSelector
{
    /// <summary>
    /// Interaction logic for TagSelector.xaml
    /// </summary>
    public partial class TagSelector : UserControl
    {
        public static readonly DependencyProperty AvailableTagsProperty =
            DependencyProperty.Register("AvailableTags", typeof (ObservableCollection<Tag>), typeof (TagSelector), new PropertyMetadata(default(ObservableCollection<Tag>)));

        public ObservableCollection<Tag> AvailableTags
        {
            get { return (ObservableCollection<Tag>) GetValue(AvailableTagsProperty); }
            set { SetValue(AvailableTagsProperty, value); }
        }
        public static readonly DependencyProperty TagsProperty =
            DependencyProperty.Register("Tags", typeof(IEnumerable), typeof(TagSelector), new PropertyMetadata(default(ObservableCollection<object>), DefaultValue));

        private static void DefaultValue(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            (dependencyObject as TagSelector).Reset(dependencyPropertyChangedEventArgs.NewValue as IEnumerable<object>);
        }

        public static readonly DependencyProperty AddTagCommandProperty =
            DependencyProperty.Register("AddTagCommand", typeof (ICommand), typeof (TagSelector), new PropertyMetadata(default(ICommand)));

        public ICommand AddTagCommand
        {
            get { return (ICommand) GetValue(AddTagCommandProperty); }
            set { SetValue(AddTagCommandProperty, value); }
        }

        protected void Reset(IEnumerable objects)
        {
            Items.CollectionChanged -= Items_CollectionChanged;
            Items.Clear();
            if (objects !=null)
            {
                foreach (var o in objects)
                {
                    Items.Add(new TagSelectorTagItem(o));
                }
            }

            Items.Add(new TagSelectorFreeTextItem(this));

            Items.CollectionChanged += Items_CollectionChanged;
        }
        
        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Tags == null)
            {
                return;
            }
            if (e.NewItems !=null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var i = newItem as TagSelectorTagItem;
                    if (i !=null)
                    {
                        AddTagCommand.Execute(i.Item);
                    }
                }
            }

            if (e.OldItems !=null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var i = oldItem as TagSelectorTagItem;
                    if (i != null)
                    {
                        //TODO: execute remove command
                    }
                }
            }
        }


        /// <summary>
        /// Collection of tags that are selected.
        /// </summary>
        public IEnumerable Tags
        {
            get { return (IEnumerable)GetValue(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }

        /// <summary>
        /// Working set of items for the control.
        /// </summary>
        public ObservableCollection<TagSelectorItem> Items { get; protected set; }

        
        public TagSelector()
        {
            Items=new ObservableCollection<TagSelectorItem>();
            Reset(null);
            InitializeComponent();
        }

        private void c_Items_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        public TagSelectorTagItem DetectAndAddTag(string txt)
        {
            foreach (var tag in AvailableTags)
            {
                if (tag.ToString().ToLower() == txt.ToLower())
                {
                    var tagwrapper = new TagSelectorTagItem(tag);

                    //TODO: check if it already exists, dont allow duplicates
                    
                    Items.Insert(Items.Count-1,tagwrapper);
                    return tagwrapper;
                }
            }

            return null;
        }
    }

    public class TagSelectorItem
    {
        
    }

    public class TagSelectorTagItem : TagSelectorItem
    {
        public TagSelectorTagItem(object item)
        {
            Item = item;
        }
        public object Item { get; set; }

    }

    public class TagSelectorFreeTextItem : TagSelectorItem, INotifyPropertyChanged,ITagDetector
    {
        private readonly TagSelector _selector;

        public TagSelectorFreeTextItem(TagSelector selector)
        {
            _selector = selector;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsValidTag(string txt)
        {
            if (_selector.DetectAndAddTag(txt) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
