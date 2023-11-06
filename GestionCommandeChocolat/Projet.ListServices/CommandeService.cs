using Newtonsoft.Json;
using Projet.FileServices;
using Projet.LogServices;
using Projet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Projet.ListServices
{
    public class CommandeService : ICommandeService

    {
        private readonly ILogService logService;

       
        public CommandeService(ILogService logService)
        {
            this.logService = logService;
        }
        public CommandeService(ILogService logService, IFileReader<Commande> fileReader, IFileWriter<Commande> fileWriter)
        {
            this.logService = logService;
          
        }
        public async Task<float> CalculerPrixTotalAsync(List<Commande> commandes, List<Article> articles)
        {
            float prixTotal = 0;

            foreach (var commande in commandes)
            {
                var article = articles.Find(a => a.Id == commande.IdChocolat);
                if (article != null)
                {
                    prixTotal += article.Prix * commande.Quantite;
                }
            }

            // Opération asynchrone factice pour satisfaire le compilateur
            await Task.CompletedTask;

            return prixTotal;
        }
        public async Task<bool> GenererFactureAsync(string acheteurNom, string acheteurPrenom, List<Commande> commandes, List<Article> articles)
        {
            try
            {
                string fileName = $"C:\\Users\\hassania\\OneDrive\\Bureau\\Nouveau dossier (5)\\{acheteurNom}-{acheteurPrenom}-{DateTime.Now:dd-MM-yyyy-HH-mm}.txt";

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
                {
                    writer.WriteLine($"Facture pour {acheteurNom} {acheteurPrenom}");
                    writer.WriteLine("Détails de la commande :");

                    foreach (var commande in commandes)
                    {
                        var article = articles.Find(a => a.Id == commande.IdChocolat);
                        if (article != null)
                        {
                            writer.WriteLine($"- {article.Reference}, Quantité : {commande.Quantite}, Prix unitaire : {article.Prix:C}, Total : {article.Prix * commande.Quantite:C}");
                        }
                    }

                    float prixTotal = await CalculerPrixTotalAsync(commandes, articles);
                    writer.WriteLine($"Prix total de la commande : {prixTotal:C}");
                }

                Console.WriteLine($"Facture générée avec succès : {fileName}");
                await logService.EnregistrerLogAsync($"Génération de facture pour {acheteurNom} {acheteurPrenom}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la génération de la facture : " + ex.Message);
                return false;
            }
        }
        public async Task<List<Article>> ObtenirArticlesVendusAsync()
        {
            try
            {
                string jsonFilePath = "C:\\ProjetC#\\GestionCommandeChocolat\\Projet.Data\\data\\articles.json"; // Remplacez par le chemin vers votre fichier JSON.

                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);

                    List<Article> articles = JsonConvert.DeserializeObject<List<Article>>(jsonContent);

                    return articles;
                }
                else
                {
                    Console.WriteLine("Le fichier JSON n'existe pas.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la lecture du fichier JSON : " + ex.Message);
            }

            //  l'opérateur 'await' pour attendre une opération asynchrone factice.
            await Task.CompletedTask;

            return new List<Article>();
        }
    }

}
