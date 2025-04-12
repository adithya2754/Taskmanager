using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Styling;
using ProcessView.Models;
using ProcessView.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProcessView.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ProcessService _processService;
        private readonly ThemeService _themeService;
        private readonly ExportService _exportService;

        // Collections
        [Reactive] public ObservableCollection<Process> Processes { get; set; }
        [Reactive] public ObservableCollection<Process> FilteredProcesses { get; set; }
        [Reactive] public ObservableCollection<ProcessClass> ProcessClasses { get; set; }
        
        // Filtering/Selection
        [Reactive] public bool ShowSystemProcesses { get; set; }
        [Reactive] public bool IsExportEnabled { get; set; }
        [Reactive] public bool IsClassAddEnabled { get; set; }
        [Reactive] public bool IsProcessSelected { get; set; }
        
        [Reactive] public ProcessClass? SelectedClass { get; set; }
        
        [Reactive] public string SearchTerm { get; set; }
        
        [Reactive] public string StatusText { get; set; }
        [Reactive] public string SelectedClassName { get; set; }
        [Reactive] public int ProcessCount { get; set; }
        [Reactive] public double SystemLoad { get; set; }
        
        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand ExportPatchCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand AddToClassCommand { get; }
        public ICommand CreateClassCommand { get; }
        public ICommand ExitCommand { get; }
        
        // Selected Processes
        public ObservableCollection<Process> SelectedProcesses { get; }
        
        public MainWindowViewModel()
        {
            // Default constructor for design-time
            Processes = new ObservableCollection<Process>();
            FilteredProcesses = new ObservableCollection<Process>();
            ProcessClasses = new ObservableCollection<ProcessClass>();
            SelectedProcesses = new ObservableCollection<Process>();
            SearchTerm = string.Empty;
            StatusText = string.Empty;
            SelectedClassName = string.Empty;
            
            // Fake services for design-time
            _processService = new ProcessService();
            _themeService = ThemeService.Instance;
            _exportService = new ExportService();
            
            // Initialize commands as no-ops for design-time
            RefreshCommand = ReactiveCommand.CreateFromTask(async () => { });
            ExportPatchCommand = ReactiveCommand.CreateFromTask(async () => { });
            ToggleThemeCommand = ReactiveCommand.Create(() => { });
            AddToClassCommand = ReactiveCommand.CreateFromTask(async () => { });
            CreateClassCommand = ReactiveCommand.CreateFromTask(async () => { });
            ExitCommand = ReactiveCommand.Create(() => { });
            
            // Load mock data for design-time
            LoadDataAsync().ConfigureAwait(false);
        }
        
        public MainWindowViewModel(ProcessService processService, ThemeService themeService, ExportService exportService)
        {
            _processService = processService;
            _themeService = themeService;
            _exportService = exportService;
            
            Processes = new ObservableCollection<Process>();
            FilteredProcesses = new ObservableCollection<Process>();
            ProcessClasses = new ObservableCollection<ProcessClass>();
            SelectedProcesses = new ObservableCollection<Process>();
            
            SearchTerm = string.Empty;
            StatusText = "Ready";
            SelectedClassName = string.Empty;
            ShowSystemProcesses = false;
            IsExportEnabled = false;
            IsClassAddEnabled = false;
            IsProcessSelected = false;
            
            // Create commands
            RefreshCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
            ExportPatchCommand = ReactiveCommand.CreateFromTask(ExportSelectedToClassAsync);
            ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);
            AddToClassCommand = ReactiveCommand.CreateFromTask(AddToClassAsync);
            CreateClassCommand = ReactiveCommand.CreateFromTask(CreateClassAsync);
            ExitCommand = ReactiveCommand.Create(Exit);
            
            // Subscribe to changes
            this.WhenAnyValue(
                x => x.SearchTerm,
                x => x.ShowSystemProcesses,
                x => x.SelectedClass
            ).Subscribe(_ => UpdateFilteredProcesses());
            
            this.WhenAnyValue(
                x => x.SelectedProcesses.Count
            ).Subscribe(count => {
                IsProcessSelected = count > 0;
                IsExportEnabled = count > 0;
                IsClassAddEnabled = count > 0;
            });
            
            this.WhenAnyValue(
                x => x.SelectedClass
            ).Subscribe(_ => UpdateStatusText());
            
            // Load data
            Task.Run(LoadDataAsync);
        }
        
        private async Task LoadDataAsync()
        {
            try
            {
                // Update status
                StatusText = "Loading...";
                
                // Get processes
                var processes = await _processService.GetProcessesAsync();
                
                // Get classes
                var classes = _processService.GetProcessClasses();
                
                // Update UI collections
                ProcessClasses.Clear();
                foreach (var c in classes)
                {
                    ProcessClasses.Add(c);
                }
                
                Processes.Clear();
                foreach (var p in processes)
                {
                    Processes.Add(p);
                }
                
                // Select "All Processes" by default if no class is selected
                if (SelectedClass == null && ProcessClasses.Any())
                {
                    SelectedClass = ProcessClasses.FirstOrDefault(c => c.Name == "All Processes");
                }
                
                // Update filtered processes
                UpdateFilteredProcesses();
                
                // Update system metrics
                ProcessCount = Processes.Count;
                SystemLoad = Processes.Sum(p => p.Cpu);
                
                // Update status
                StatusText = "Ready";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                StatusText = $"Error: {ex.Message}";
            }
            
            return;
        }
        
        private void UpdateStatusText()
        {
            if (SelectedClass == null)
            {
                StatusText = "Ready";
                return;
            }
            
            string className = SelectedClass.Name;
            bool isSystemClass = SelectedClass.IsSystemGenerated;
            
            if (isSystemClass)
            {
                StatusText = $"Viewing {className}";
            }
            else
            {
                StatusText = $"Viewing class: {className} ({SelectedClass.Processes.Count} processes)";
            }
        }
        
        private async void UpdateFilteredProcesses()
        {
            try
            {
                var filteredProcesses = await _processService.GetFilteredProcessesAsync(
                    SearchTerm, 
                    ShowSystemProcesses, 
                    SelectedClass);
                
                FilteredProcesses.Clear();
                
                foreach (var process in filteredProcesses)
                {
                    FilteredProcesses.Add(process);
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }
        
        private void ToggleTheme()
        {
            _themeService.ToggleTheme();
        }
        
        private bool ValidateClassName(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                StatusText = "Error: Class name cannot be empty";
                return false;
            }
            
            if (_processService.GetProcessClassByName(className) != null)
            {
                StatusText = $"Error: A class named '{className}' already exists";
                return false;
            }
            
            return true;
        }
        
        private async Task<string> CreateClassAsync()
        {
            try
            {
                // For a real app, we'd show a dialog here
                string className = "New Class " + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string description = "User created class";
                
                bool success = await _processService.CreateClassAsync(className, description);
                
                if (success)
                {
                    StatusText = $"Created new class: {className}";
                    
                    // Refresh to show the new class
                    await LoadDataAsync();
                    
                    return className;
                }
                else
                {
                    StatusText = "Error: Could not create class";
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating class: {ex.Message}");
                StatusText = $"Error: {ex.Message}";
                return string.Empty;
            }
        }
        
        private async Task AddToClassAsync()
        {
            if (SelectedProcesses.Count == 0)
            {
                StatusText = "Please select one or more processes";
                return;
            }
            
            // For a real app, we'd show a dialog here
            // with a list of classes to choose from
            string className = await CreateClassAsync();
            
            if (string.IsNullOrEmpty(className))
            {
                return;
            }
            
            try
            {
                var processes = SelectedProcesses.ToList();
                
                await _processService.AddToClassAsync(className, processes);
                
                StatusText = $"Added {processes.Count} processes to class: {className}";
                
                // Refresh data to show updated class membership
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to class: {ex.Message}");
                StatusText = $"Error: {ex.Message}";
            }
        }
        
        private async Task ExportSelectedToClassAsync()
        {
            if (SelectedProcesses.Count == 0 || SelectedClass == null)
            {
                StatusText = "Please select processes and a class to export";
                return;
            }
            
            var className = SelectedClass.Name;
            var createdBy = Environment.UserName;
            
            try
            {
                var filePath = await _exportService.ExportPatchAsync(
                    className,
                    createdBy,
                    SelectedProcesses.ToList());
                
                if (!string.IsNullOrEmpty(filePath))
                {
                    StatusText = $"Exported patch to: {filePath}";
                }
                else
                {
                    StatusText = "Error: Failed to export patch";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting patch: {ex.Message}");
                StatusText = $"Error: {ex.Message}";
            }
        }
        
        private void Exit()
        {
            // For a real app, we would check for unsaved changes here
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}