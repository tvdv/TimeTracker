using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using TimeTracker.Properties;
using TimeTracker.TimeEntryView;
using TimeTracker.util;

namespace TimeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Model.Model _model;
        private readonly ViewModel _viewModel;
        public MainWindow()
        {

            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TimeTracker");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            _model = new Model.Model(dir, this.Dispatcher);
            DataContext = _viewModel = new ViewModel(_model,this.Dispatcher);

            InitializeComponent();

            //_viewModel.RefreshView();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _model.BeginLoad();
        }

        private bool _closeSaveFinished = false;
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!_closeSaveFinished)
            {
                _model.PropertyChanged += _model_PropertyChanged;
                _model.BeginSave();
                e.Cancel = true;
            }
            else
            {
                var placement = WindowPlacement.GetWindowsPlacement(this);
                Settings.Default.WndPlacement = SerializePlacement(placement);
                Settings.Default.Save();    
            }
        }

        protected String SerializePlacement(WindowPlacement.WINDOWPLACEMENT placement)
        {
            Encoding encoding = new UTF8Encoding();
            var serializer = new XmlSerializer(typeof(WindowPlacement.WINDOWPLACEMENT));
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                {
                    serializer.Serialize(xmlTextWriter, placement);
                    byte[] xmlBytes = memoryStream.ToArray();
                    return encoding.GetString(xmlBytes);
                }
            }
        }

        protected WindowPlacement.WINDOWPLACEMENT? DeserialisePlacement(string xml)
        {
            Encoding encoding = new UTF8Encoding();
            var serializer = new XmlSerializer(typeof(WindowPlacement.WINDOWPLACEMENT));
            WindowPlacement.WINDOWPLACEMENT placement;
            try
            {
                byte[] xmlBytes = encoding.GetBytes(xml);
                using (var memoryStream = new MemoryStream(xmlBytes))
                {
                    placement = (WindowPlacement.WINDOWPLACEMENT)serializer.Deserialize(memoryStream);
                }
                return placement;
            }
            catch (InvalidOperationException)
            {
                return null;
            }

        }

        void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State" && _model.State==Model.Model.ModelState.Loaded)
            {
                _closeSaveFinished = true;
                Close();
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var ecv = EntriesList.Items as IEditableCollectionView;
            foreach (var removedItem in e.RemovedItems)
            {
                
                ecv.EditItem(removedItem);
                ecv.CommitEdit();    
            }
            
            
        }

        private void MainWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            var placement = DeserialisePlacement(Settings.Default.WndPlacement);
            if (placement.HasValue)
            {
                WindowPlacement.SetWindowPlacement(this, placement.Value);
            }
        }
    }
}
