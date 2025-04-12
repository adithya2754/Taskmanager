using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ProcessView.ViewModels;

namespace ProcessView
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data == null)
                return new TextBlock { Text = "Data is null" };
                
            try
            {
                var name = data.GetType().FullName?.Replace("ViewModel", "View") ?? "Unknown";
                var type = Type.GetType(name);

                if (type != null)
                {
                    return (Control)Activator.CreateInstance(type)!;
                }
                
                return new TextBlock { Text = $"Could not find view for {name}" };
            }
            catch (Exception ex)
            {
                return new TextBlock { Text = $"Error finding view: {ex.Message}" };
            }
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}