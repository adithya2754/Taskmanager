using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProcessView.ViewModels;
using ProcessView.Models;
using System.Linq;

namespace ProcessView.Views
{
    public partial class MainWindow : Window
    {
        private DataGrid? _processesGrid;
        private MainWindowViewModel? _viewModel;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
            // Wait for Loaded to setup DataGrid
            this.Loaded += MainWindow_Loaded;
        }
        
        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            // Get a reference to the DataGrid once the window is loaded
            _processesGrid = this.FindControl<DataGrid>("ProcessesGrid");
            
            // Get the ViewModel
            _viewModel = DataContext as MainWindowViewModel;
            
            // Hook up the selection changed event
            if (_processesGrid != null)
            {
                _processesGrid.SelectionChanged += ProcessesGrid_SelectionChanged;
            }
        }

        private void ProcessesGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && _processesGrid != null)
            {
                // Clear the current selection
                _viewModel.SelectedProcesses.Clear();
                
                // Add all selected items
                foreach (Process process in _processesGrid.SelectedItems)
                {
                    _viewModel.SelectedProcesses.Add(process);
                }
            }
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            
            // Update the ViewModel reference when DataContext changes
            _viewModel = DataContext as MainWindowViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}