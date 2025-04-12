using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProcessView.Models;

namespace ProcessView.Services
{
    public class ProcessService
    {
        private List<Process> _processes;
        private List<ProcessClass> _processClasses;
        
        public ProcessService()
        {
            _processes = new List<Process>();
            _processClasses = new List<ProcessClass>();
            
            // Initially load mock data
            LoadMockData();
        }
        
        private void LoadMockData()
        {
            _processes = MockData.GetMockProcesses();
            _processClasses = MockData.GetMockProcessClasses();
            
            // Make sure all processes know their classes (in case mock data wasn't connected properly)
            foreach (var process in _processes)
            {
                foreach (var processClass in _processClasses)
                {
                    if (processClass.Processes.Contains(process.Name) || 
                        processClass.ProcessPaths.Contains(process.Path))
                    {
                        if (!process.Classes.Contains(processClass))
                        {
                            process.Classes.Add(processClass);
                        }
                    }
                }
            }
        }
        
        public async Task<List<Process>> GetProcessesAsync()
        {
            // Simulate a load delay
            await Task.Delay(50);
            return _processes;
        }
        
        public async Task<List<Process>> GetProcessesAsync(ProcessClass processClass)
        {
            // Simulate a load delay
            await Task.Delay(50);
            
            if (processClass.Name == "All Processes")
            {
                return _processes;
            }
            
            if (processClass.Name == "Favorites")
            {
                return _processes.Where(p => p.IsFavorite).ToList();
            }
            
            return _processes.Where(p => p.Classes.Any(c => c.Id == processClass.Id)).ToList();
        }
        
        public List<string> GetClassNames()
        {
            return _processClasses.Select(pc => pc.Name).ToList();
        }
        
        public async Task<List<Process>> GetFilteredProcessesAsync(string searchTerm, bool showSystemProcesses, ProcessClass? selectedClass)
        {
            var processes = selectedClass != null ? 
                await GetProcessesAsync(selectedClass) : 
                await GetProcessesAsync();
            
            var filteredProcesses = processes
                .Where(p => showSystemProcesses || !p.IsSystem)
                .ToList();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                filteredProcesses = filteredProcesses
                    .Where(p => p.Name.ToLower().Contains(searchTerm) || 
                                p.Path.ToLower().Contains(searchTerm) ||
                                p.User.ToLower().Contains(searchTerm))
                    .ToList();
            }
            
            return filteredProcesses;
        }
        
        public List<ProcessClass> GetProcessClasses()
        {
            return _processClasses;
        }
        
        public ProcessClass? GetProcessClassByName(string name)
        {
            return _processClasses.FirstOrDefault(pc => pc.Name == name);
        }
        
        public async Task<bool> CreateClassAsync(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name) || _processClasses.Any(pc => pc.Name == name))
            {
                return false;
            }
            
            var newClass = new ProcessClass
            {
                Id = _processClasses.Max(pc => pc.Id) + 1,
                Name = name,
                Description = description,
                CpuUsage = 0,
                ProcessCount = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = Environment.UserName,
                IsSystemGenerated = false
            };
            
            _processClasses.Add(newClass);
            
            return true;
        }
        
        public async Task AddToClassAsync(string className, List<Process> processes)
        {
            var processClass = GetProcessClassByName(className);
            
            if (processClass == null || processes == null || processes.Count == 0)
            {
                return;
            }
            
            foreach (var process in processes)
            {
                if (!process.Classes.Contains(processClass))
                {
                    process.Classes.Add(processClass);
                }
                
                if (!processClass.Processes.Contains(process.Name))
                {
                    processClass.Processes.Add(process.Name);
                }
                
                if (!processClass.ProcessPaths.Contains(process.Path))
                {
                    processClass.ProcessPaths.Add(process.Path);
                }
            }
            
            processClass.ProcessCount = processClass.Processes.Count;
            processClass.CpuUsage = processes.Sum(p => p.Cpu);
        }
        
        public bool ToggleFavorite(Process process)
        {
            process.IsFavorite = !process.IsFavorite;
            return process.IsFavorite;
        }
    }
}