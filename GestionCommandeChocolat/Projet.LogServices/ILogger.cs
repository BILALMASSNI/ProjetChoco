using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.LogServices
{
    public interface ILogService
    {
        Task EnregistrerLogAsync(string message);
        Task<List<string>> LireLogsAsync();
}
}