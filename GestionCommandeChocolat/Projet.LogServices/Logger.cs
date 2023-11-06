using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.LogServices
{
    public class LogService : ILogService
    {
        private readonly List<string> logs;
        private const string logFilePath = "C:\\Users\\hassania\\OneDrive\\Bureau\\Nouveau dossier (5)\\application.log"; 

        public LogService()
        {
            logs = new List<string>();
        }

        public async Task EnregistrerLogAsync(string message)
        {
            string logEntry = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {message}";
            logs.Add(logEntry);

            // Enregistrement  de log dans un fichier.
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                await writer.WriteLineAsync(logEntry);
            }
        }

        public Task<List<string>> LireLogsAsync()
        {
            return Task.FromResult(logs);
        }
    }

}