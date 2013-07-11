using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

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
            DataContext = _viewModel = new ViewModel(_model);

            InitializeComponent();

            _viewModel.RefreshView();
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
        }

        void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State" && _model.State==Model.Model.ModelState.Loaded)
            {
                _closeSaveFinished = true;
                Close();
            }
        }
    }
}
