using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.ReactiveUI;
using ProcessView.ViewModels;
using ProcessView.Views;
using System.Diagnostics;

namespace ProcessView
{
    class Program
    {
        // The main entry point for the application.
        public static void Main(string[] args)
        {
            // Set up thread exception handling
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.Error.WriteLine($"Unhandled exception: {e.ExceptionObject}");
            };
            
            // Start the application based on environment
            if (IsHeadlessMode())
            {
                Console.WriteLine("Running in headless mode detected");
                RunConsoleMode();
            }
            else
            {
                // Enable shadow copying to prevent PDB file locking issues
                try
                {
                    AppDomain.CurrentDomain.SetShadowCopyFiles();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Warning: Could not set shadow copy files: {ex.Message}");
                }
                
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        // Runs the application in console mode for headless environments
        private static void RunConsoleMode()
        {
            Console.WriteLine("ProcessView Console Mode");
            Console.WriteLine("========================");
            
            try
            {
                var processService = new Services.ProcessService();
                processService.Initialize(true);
                
                var processes = processService.GetProcessesAsync().Result;
                Console.WriteLine($"Found {processes.Count} processes:");
                foreach (var process in processes)
                {
                    Console.WriteLine($"- {process.Name} (PID: {process.Pid}) - CPU: {process.Cpu}%, Memory: {process.Memory} MB");
                }
                
                var classes = processService.GetProcessClassesAsync().Result;
                Console.WriteLine($"Found {classes.Count} process classes:");
                foreach (var processClass in classes)
                {
                    Console.WriteLine($"- {processClass.Name} ({processClass.Processes.Count} processes)");
                }
                
                Console.WriteLine("Running in headless mode. Application will exit when process is terminated.");
                
                // In headless mode, just keep the application running until it's terminated
                ManualResetEvent waitHandle = new ManualResetEvent(false);
                Console.CancelKeyPress += (sender, e) => 
                {
                    waitHandle.Set();
                    e.Cancel = true;
                };
                
                waitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in console mode: {ex}");
            }
        }

        // Check if we're running in headless mode
        private static bool IsHeadlessMode()
        {
            try
            {
                // Check for specific environment variable
                string headlessEnv = Environment.GetEnvironmentVariable("HEADLESS");
                if (!string.IsNullOrEmpty(headlessEnv) && 
                    (headlessEnv == "1" || headlessEnv.ToLower() == "true"))
                {
                    return true;
                }
                
                // Check if we are on Linux and running without a display
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    string display = Environment.GetEnvironmentVariable("DISPLAY");
                    if (string.IsNullOrEmpty(display))
                    {
                        return true;
                    }
                }
                
                return false;
            }
            catch
            {
                // If there's any error, assume we're not in headless mode
                return false;
            }
        }
    }
}