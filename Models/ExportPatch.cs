using System;
using System.Collections.Generic;

namespace ProcessView.Models
{
    public class ProcessExport
    {
        public int Pid { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Cpu { get; set; }
        public double Memory { get; set; }
        public DateTime StartTime { get; set; }
    }
    
    public class ExportPatch
    {
        public string ClassName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public List<ProcessExport> Processes { get; set; } = new List<ProcessExport>();
    }
}