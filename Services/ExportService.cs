using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProcessView.Models;

namespace ProcessView.Services
{
    public class ExportService
    {
        private string ExportDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
        
        public async Task<string> ExportPatchAsync(string className, string createdBy, List<Process> processes)
        {
            try
            {
                // Ensure export directory exists
                if (!Directory.Exists(ExportDirectory))
                {
                    Directory.CreateDirectory(ExportDirectory);
                }
                
                // Create export data
                var exportData = new ExportPatch
                {
                    ClassName = className,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                    Processes = processes.Select(p => new ProcessExport
                    {
                        Pid = p.Pid,
                        Name = p.Name,
                        Cpu = p.Cpu,
                        Memory = p.Memory,
                        StartTime = p.StartTime
                    }).ToList()
                };
                
                // Generate file path
                string filePath = Path.Combine(ExportDirectory, $"{className}.patch.json");
                
                // Serialize and save
                string jsonData = JsonConvert.SerializeObject(exportData, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, jsonData);
                
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting patch: {ex.Message}");
                return string.Empty;
            }
        }
        
        public async Task<string> ExportProcessDetailsAsync(Process process)
        {
            try
            {
                // Ensure export directory exists
                if (!Directory.Exists(ExportDirectory))
                {
                    Directory.CreateDirectory(ExportDirectory);
                }
                
                // Generate file path
                string filePath = Path.Combine(ExportDirectory, $"{process.Name}_{process.Pid}.json");
                
                // Serialize and save
                string jsonData = JsonConvert.SerializeObject(process, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, jsonData);
                
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting process details: {ex.Message}");
                return string.Empty;
            }
        }
        
        public async Task<List<string>> GetExistingExportsAsync()
        {
            try
            {
                string exportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
                
                if (!Directory.Exists(exportDir))
                {
                    Directory.CreateDirectory(exportDir);
                }
                
                List<string> exportFiles = new List<string>();
                
                foreach (string file in Directory.GetFiles(exportDir, "*.patch.json"))
                {
                    exportFiles.Add(Path.GetFileName(file));
                }
                
                return exportFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing exports: {ex.Message}");
                return new List<string>();
            }
        }
        
        public async Task<string> GetExportPathAsync()
        {
            return ExportDirectory;
        }
    }
}