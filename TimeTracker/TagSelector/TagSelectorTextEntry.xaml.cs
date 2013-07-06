using System;
using System.Collections.Generic;
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

namespace TimeTracker.TagSelector
{
    /// <summary>
    /// Interaction logic for TagSelectorTextEntry.xaml
    /// </summary>
    public partial class TagSelectorTextEntry : UserControl
    {
        public static readonly DependencyProperty TagSelectorFreeTextItemProperty =
            DependencyProperty.Register("TagSelectorFreeTextItem", typeof(TagSelectorFreeTextItem), typeof(TagSelectorTextEntry), new PropertyMetadata(null));

        public static readonly DependencyProperty TagDetectorProperty =
            DependencyProperty.Register("TagDetector", typeof (ITagDetector), typeof (TagSelectorTextEntry), new PropertyMetadata(default(ITagDetector)));

        public ITagDetector TagDetector
        {
            get { return (ITagDetector) GetValue(TagDetectorProperty); }
            set { SetValue(TagDetectorProperty, value); }
        }


        public TagSelectorFreeTextItem TagSelectorFreeTextItem
        {
            get { return (TagSelectorFreeTextItem)GetValue(TagSelectorFreeTextItemProperty); }
            set { SetValue(TagSelectorFreeTextItemProperty, value); }
        }
        public TagSelectorTextEntry()
        {
            InitializeComponent();
        }


 

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                var tb = ((TextBox)sender);

                var word = tb.Text.Substring(0, tb.SelectionStart);
                if (TagDetector.IsValidTag(word))
                {
                    //remove word
                    if (tb.SelectionStart >= tb.Text.Length)
                    {
                        tb.Text = "";
                    }
                    else
                    {
                        tb.Text = tb.Text.Substring(tb.SelectionStart+1);
                    }
                }
            }
        }
    }

    public interface ITagDetector
    {
         bool IsValidTag(string txt);
    }
}
