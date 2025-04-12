using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ProcessView.ViewModels;
using ProcessView.Views;
using ProcessView.Services;

namespace ProcessView
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var processService = new ProcessService();
                var themeService = ThemeService.Instance;
                var exportService = new ExportService();
                
                var mainViewModel = new MainWindowViewModel(processService, themeService, exportService);
                
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}