using System;
using Avalonia.Styling;

namespace ProcessView.Services
{
    public class ThemeService
    {
        // Initialize the instance field to avoid nullability warning
        private static ThemeService _instance = new ThemeService();
        
        private ThemeVariant _currentTheme = ThemeVariant.Default;
        private ThemeMode _currentMode = ThemeMode.System;
        
        // Initialize the event to avoid nullability warning
        public event EventHandler<ThemeVariant?> ThemeChanged = delegate { };
        
        public ThemeVariant CurrentTheme => _currentTheme;
        public ThemeMode CurrentMode => _currentMode;
        
        public static ThemeService Instance
        {
            get
            {
                return _instance;
            }
        }
        
        private ThemeService()
        {
            // Private constructor for singleton pattern
        }
        
        public void ToggleTheme()
        {
            // Toggle between Light and Dark modes
            if (_currentMode == ThemeMode.Light)
            {
                ChangeTheme(ThemeMode.Dark);
            }
            else
            {
                ChangeTheme(ThemeMode.Light);
            }
        }
        
        public void ChangeTheme(ThemeMode mode)
        {
            _currentMode = mode;
            
            switch (mode)
            {
                case ThemeMode.Light:
                    _currentTheme = ThemeVariant.Light;
                    break;
                case ThemeMode.Dark:
                    _currentTheme = ThemeVariant.Dark;
                    break;
                default:
                    _currentTheme = ThemeVariant.Default;
                    break;
            }
            
            ThemeChanged?.Invoke(this, _currentTheme);
        }
    }
}