using Projet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Projet.FileServices
{
    public class FileReader<T> : IFileReader<T>
    {
        public async Task<List<T>> ReadDataFromFileAsync(string filePath)
        {
            List<T> data = new List<T>();

            try
            {
                //  si le fichier JSON existe
                if (File.Exists(filePath))
                {
                    // Lecture le contenu du fichier JSON de manière asynchrone
                    string jsonContent = await Task.Run(() => File.ReadAllText(filePath));

                    // Désérialisation de le contenu en une liste d'objets de type T
                    data = JsonSerializer.Deserialize<List<T>>(jsonContent);

                    // Retournez la liste des objets
                    return data;
                }
                else
                {
                    // Le fichier n'existe pas retourne d'une liste vide .
                    return data;
                }
            }
            catch (Exception ex)
            {
                //  erreur de lecture du fichier JSON 
                Console.WriteLine("Erreur de lecture du fichier JSON : " + ex.Message);
                return data;
            }
        }

    }


}