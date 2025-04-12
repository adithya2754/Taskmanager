using System;
using System.Collections.Generic;

namespace ProcessView.Models
{
    public class Process
    {
        public string Id { get; set; }
        public int Pid { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public DateTime StartTime { get; set; }
        public string User { get; set; }
        public int ThreadCount { get; set; }
        public int Priority { get; set; }
        
        public List<string> ModulesLoaded { get; set; }
        public bool IsSystem { get; set; }
        public bool CanTerminate { get; set; }
        
        // Added properties that were missing
        public double Cpu { get; set; } // CPU usage percentage
        public string Path { get; set; } // Process executable path
        public bool IsFavorite { get; set; } // Flag for favorite processes
        public List<ProcessClass> Classes { get; set; } // Process classes this process belongs to
        
        // Additional missing properties
        public double Memory { get; set; } // Memory usage in MB
        public int Threads { get; set; } // Number of threads
        public int Handles { get; set; } // Number of handles
        
        public Process()
        {
            Id = Guid.NewGuid().ToString();
            Name = "";
            Status = "";
            User = "";
            Path = "";
            ModulesLoaded = new List<string>();
            Classes = new List<ProcessClass>();
            IsFavorite = false;
            Memory = 0;
            Threads = 0;
            Handles = 0;
        }
    }
}