using Projet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.FileServices
{
    public interface IFileReader<T>
    {
        Task<List<T>> ReadDataFromFileAsync(string filePath);

    }
 
}





