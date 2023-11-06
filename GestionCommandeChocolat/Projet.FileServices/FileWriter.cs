using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Projet.FileServices
{
    public class FileWriter<T> : IFileWriter<T>
    {
        public async Task WriteDataToFileAsync(string filePath, List<T> data)
        {

            try
            {
                // Sérialisation de la liste d'objets en format JSON
                string jsonContent = JsonSerializer.Serialize(data);

                // Écriture de contenu JSON dans le fichier de manière asynchrone
                await Task.Run(() => File.WriteAllText(filePath, jsonContent));


                Console.WriteLine("Données écrites avec succès dans le fichier JSON.");
            }
            catch (Exception ex)
            {
                //  erreur d'écriture du fichier JSON 
                Console.WriteLine("Erreur d'écriture du fichier JSON : " + ex.Message);
            }
        }
    }
}
