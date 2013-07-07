using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TimeTracker.TimeEntryView;
using TimeTracker.util;

namespace TimeTracker.TagSelector
{
    /// <summary>
    /// Interaction logic for TagSelector.xaml
    /// </summary>
    public partial class TagSelector : UserControl,INotifyPropertyChanged
    {
        public static readonly DependencyProperty AvailableTagsProperty =
            DependencyProperty.Register("AvailableTags", typeof (IEnumerable), typeof (TagSelector), new PropertyMetadata(default(ObservableCollection<Tag>)));

        public IEnumerable AvailableTags
        {
            get { return (IEnumerable)GetValue(AvailableTagsProperty); }
            set { SetValue(AvailableTagsProperty, value); }
        }

        public static readonly DependencyProperty TagsProperty =
            DependencyProperty.Register("Tags", typeof(ObservableCollection<TimeEntryTagAssociationViewModel>), typeof(TagSelector), new PropertyMetadata(default(ObservableCollection<object>), DefaultValue));

        private static void DefaultValue(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            (dependencyObject as TagSelector).Reset(dependencyPropertyChangedEventArgs.NewValue as ObservableCollection<TimeEntryTagAssociationViewModel>);
        }

        public static readonly DependencyProperty AddTagCommandProperty =
            DependencyProperty.Register("AddTagCommand", typeof (ICommand), typeof (TagSelector), new PropertyMetadata(default(ICommand)));

        public ICommand AddTagCommand
        {
            get { return (ICommand) GetValue(AddTagCommandProperty); }
            set { SetValue(AddTagCommandProperty, value); }
        }

        protected void Reset(ObservableCollection<TimeEntryTagAssociationViewModel> objects)
        {

            _items = new VmObservableCollection<TagSelectorItem, TimeEntryTagAssociationViewModel>(objects, o => new TagSelectorTagItem(o), (o, item)
                                                                                                             =>
                {
                    var tagselItem = item as TagSelectorTagItem;
                    if (tagselItem != null)
                    {
                        return tagselItem.Item == o;
                    }
                    else
                    {
                        return false;
                    }
                });
           

            _items.Add(new TagSelectorFreeTextItem(this));

            OnPropertyChanged("Items");
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
        /// TOOD: How to make using a generic type but Tags still be an ObservableCollection???
        /// </summary>
        public VmObservableCollection<TagSelectorItem, TimeEntryTagAssociationViewModel> Items
        {
            get { return _items; }
        }


        private VmObservableCollection<TagSelectorItem, TimeEntryTagAssociationViewModel> _items;

        public TagSelector()
        {
            _items = null;
            InitializeComponent();
        }

        private void c_Items_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (c_Items.SelectedItem != null)
                {
                    
                }
            }
        }

        public object DetectAndAddTag(string txt)
        {
            foreach (var tag in AvailableTags)
            {
                if (tag.ToString().ToLower() == txt.ToLower())
                {
                    AddTagCommand.Execute(tag);

                    return tag;
                }
            }

            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); 
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
