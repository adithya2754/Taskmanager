using System;
using System.Collections.Generic;

namespace ProcessView.Models
{
    public static class MockData
    {
        public static List<Process> GetMockProcesses()
        {
            var mockData = new List<Process>
            {
                new Process
                {
                    Id = "proc1",
                    Pid = 1234,
                    Name = "chrome.exe",
                    User = "User",
                    Cpu = 12.3,
                    Memory = 250.7,
                    StartTime = DateTime.Now.AddHours(-2),
                    Path = "/usr/bin/chrome",
                    Threads = 47,
                    Handles = 102,
                    Classes = new List<ProcessClass> { GetBrowsersClass() }
                },
                new Process
                {
                    Id = "proc2",
                    Pid = 5678,
                    Name = "notepad.exe",
                    User = "User",
                    Cpu = 0.1,
                    Memory = 15.2,
                    StartTime = DateTime.Now.AddHours(-1),
                    Path = "/usr/bin/notepad",
                    Threads = 5,
                    Handles = 21,
                    Classes = new List<ProcessClass>()
                },
                new Process
                {
                    Id = "proc3",
                    Pid = 9012,
                    Name = "firefox.exe",
                    User = "User",
                    Cpu = 8.5,
                    Memory = 180.5,
                    StartTime = DateTime.Now.AddMinutes(-45),
                    Path = "/usr/bin/firefox",
                    Threads = 36,
                    Handles = 85,
                    Classes = new List<ProcessClass> { GetBrowsersClass() }
                },
                new Process
                {
                    Id = "proc4",
                    Pid = 3456,
                    Name = "code.exe",
                    User = "User",
                    Cpu = 5.2,
                    Memory = 350.1,
                    StartTime = DateTime.Now.AddMinutes(-120),
                    Path = "/usr/bin/code",
                    Threads = 22,
                    Handles = 67,
                    Classes = new List<ProcessClass>()
                },
                new Process
                {
                    Id = "proc5",
                    Pid = 7890,
                    Name = "spotify.exe",
                    User = "User",
                    Cpu = 3.7,
                    Memory = 120.8,
                    StartTime = DateTime.Now.AddMinutes(-30),
                    Path = "/usr/bin/spotify",
                    Threads = 18,
                    Handles = 54,
                    Classes = new List<ProcessClass>()
                },
                new Process
                {
                    Id = "proc6",
                    Pid = 1357,
                    Name = "steam.exe",
                    User = "User",
                    Cpu = 1.9,
                    Memory = 80.3,
                    StartTime = DateTime.Now.AddHours(-3),
                    Path = "/usr/bin/steam",
                    Threads = 15,
                    Handles = 46,
                    Classes = new List<ProcessClass> { GetGamesClass() }
                },
                new Process
                {
                    Id = "proc7",
                    Pid = 2468,
                    Name = "game.exe",
                    User = "User",
                    Cpu = 22.4,
                    Memory = 512.0,
                    StartTime = DateTime.Now.AddMinutes(-15),
                    Path = "/usr/bin/game",
                    Threads = 28,
                    Handles = 93,
                    Classes = new List<ProcessClass> { GetGamesClass() }
                },
                new Process
                {
                    Id = "proc8",
                    Pid = 8642,
                    Name = "explorer.exe",
                    User = "System",
                    Cpu = 0.3,
                    Memory = 45.6,
                    StartTime = DateTime.Now.AddHours(-5),
                    Path = "/usr/bin/explorer",
                    Threads = 12,
                    Handles = 32,
                    Classes = new List<ProcessClass>(),
                    IsSystem = true
                },
                new Process
                {
                    Id = "proc9",
                    Pid = 9753,
                    Name = "system.exe",
                    User = "System",
                    Cpu = 0.2,
                    Memory = 28.4,
                    StartTime = DateTime.Now.AddHours(-5),
                    Path = "/usr/bin/system",
                    Threads = 8,
                    Handles = 25,
                    Classes = new List<ProcessClass>(),
                    IsSystem = true
                }
            };

            return mockData;
        }

        public static List<ProcessClass> GetMockProcessClasses()
        {
            return new List<ProcessClass>
            {
                new ProcessClass
                {
                    Id = 1,
                    Name = "All Processes",
                    Description = "Shows all processes",
                    CpuUsage = 55.1,
                    ProcessCount = 9,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    CreatedBy = "System",
                    IsSystemGenerated = true
                },
                new ProcessClass
                {
                    Id = 2,
                    Name = "Favorites",
                    Description = "User favorite processes",
                    CpuUsage = 0,
                    ProcessCount = 0,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    CreatedBy = "System",
                    IsSystemGenerated = true
                },
                GetBrowsersClass(),
                GetGamesClass()
            };
        }

        private static ProcessClass GetBrowsersClass()
        {
            return new ProcessClass
            {
                Id = 3,
                Name = "Browsers",
                Description = "Web browser processes",
                CpuUsage = 20.8,
                ProcessCount = 2,
                CreatedAt = DateTime.Now.AddDays(-15),
                CreatedBy = "User",
                Processes = new List<string> { "chrome.exe", "firefox.exe" },
                ProcessPaths = new List<string> { "/usr/bin/chrome", "/usr/bin/firefox" },
                IsSystemGenerated = false
            };
        }

        private static ProcessClass GetGamesClass()
        {
            return new ProcessClass
            {
                Id = 4,
                Name = "Games",
                Description = "Gaming related processes",
                CpuUsage = 24.3,
                ProcessCount = 2,
                CreatedAt = DateTime.Now.AddDays(-7),
                CreatedBy = "User",
                Processes = new List<string> { "steam.exe", "game.exe" },
                ProcessPaths = new List<string> { "/usr/bin/steam", "/usr/bin/game" },
                IsSystemGenerated = false
            };
        }
    }
}