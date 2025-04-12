using System;
using System.Collections.Generic;

namespace ProcessView.Models
{
    public class ProcessClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double CpuUsage { get; set; }
        public int ProcessCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsSystemGenerated { get; set; }
        
        // Process names that belong to this class
        public List<string> Processes { get; set; } = new List<string>();
        
        // Paths that belong to this class (for path-based matching)
        public List<string> ProcessPaths { get; set; } = new List<string>();
        
        public ProcessClass()
        {
            Name = string.Empty;
            Description = string.Empty;
            CreatedBy = string.Empty;
            Processes = new List<string>();
            ProcessPaths = new List<string>();
        }
    }
}