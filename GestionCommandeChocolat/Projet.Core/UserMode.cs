using Newtonsoft.Json;
using Projet.ListServices;
using Projet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Projet.Core
{
    public class UserMode
    {
        private readonly IFactureService factureService;
        private readonly ArticleService articleService;

        public UserMode(IFactureService factureService, ArticleService articleService)
        {
            this.factureService = factureService;
            this.articleService = articleService;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Mode utilisateur activé.");
            Console.Write("Veuillez entrer votre nom : ");
            string nom = Console.ReadLine();
            Console.Write("Veuillez entrer votre prénom : ");
            string prenom = Console.ReadLine();
            Console.Write("Veuillez entrer votre adresse : ");
            string adresse = Console.ReadLine();
            Console.Write("Veuillez entrer votre téléphone : ");
            int telephone = int.Parse(Console.ReadLine());

            Acheteur acheteur = new Acheteur(Guid.NewGuid(), nom, prenom, adresse, telephone);

            // Chargez les données JSON actuelles à partir du fichier
            List<Acheteur> acheteurs = LoadAcheteursFromJsonFile();

            // Ajoutez le nouvel acheteur à la liste
            acheteurs.Add(acheteur);

            // Enregistrez les données mises à jour dans le fichier JSON
            SaveAcheteursToJsonFile(acheteurs);

            List<Facture> commandes = new List<Facture>();
            var articles = await articleService.GetArticlesAsync();



            while (true)
            {
                Console.WriteLine("Saisissez la référence de l'article ou tapez 'F' pour finir la commande ou 'P' pour voir le prix actuel :");
                string choix = Console.ReadLine();

                if (choix == "F" || choix == "f")
                {
                    break;  // L'utilisateur a terminé la commande, sortez de la boucle.
                }
                else if (choix == "P" || choix == "p")
                {
                    float total = 0;
                    Console.WriteLine("Récapitulatif de la commande en cours :");
                    foreach (var facture in commandes)
                    {
                        Console.WriteLine($"Article : {facture.NomAcheteur} {facture.PrenomAcheteur} - {facture.PrixTotal:C}");
                        total += facture.PrixTotal;
                    }
                    Console.WriteLine($"Prix total actuel : {total:C}");
                }
                else
                {
                    // Recherche de l'article par sa référence
                    var articleChoisi = articles.FirstOrDefault(a => a.Reference.Equals(choix, StringComparison.OrdinalIgnoreCase));

                    if (articleChoisi != null)
                    {
                        Console.Write("Veuillez entrer la quantité : ");
                        int quantite = int.Parse(Console.ReadLine());

                        Facture facture = new Facture(Guid.NewGuid(), DateTime.Now, "Facture d'achat", acheteur.Nom, acheteur.Prenom);
                        facture.Articles.Add(articleChoisi);
                        facture.PrixTotal = articleChoisi.Prix * quantite;
                        commandes.Add(facture);



                        Console.WriteLine($"Article ajouté à la commande.");

             
                    }
                    else
                    {
                        Console.WriteLine("Référence d'article invalide. Veuillez entrer une référence valide.");
                    }
                }
            }


            if (commandes.Count > 0)
            {
                // Générer un fichier texte avec le récapitulatif de la commande
                string fileName = $"C:\\Users\\hassania\\OneDrive\\Bureau\\Nouveau dossier (5)\\{acheteur.Nom}-{acheteur.Prenom}-{DateTime.Now:dd-MM-yyyy-HH-mm}.txt";
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.WriteLine($"Récapitulatif de la commande pour {acheteur.Nom} {acheteur.Prenom}");
                    writer.WriteLine($"Date : {DateTime.Now:dd-MM-yyyy HH:mm}");
                    writer.WriteLine("Articles commandés :");
                    foreach (var facture in commandes)
                    {
                        writer.WriteLine($"Facture pour {facture.NomAcheteur} {facture.PrenomAcheteur}");
                        writer.WriteLine("Articles commandés :");
                        foreach (var article in facture.Articles)
                        {
                            writer.WriteLine($"Reference  :{article.Reference} - Prix unitaire : {article.Prix:C}");
                        }
                        writer.WriteLine($"Quantité : {facture.Articles.Count}");
                        writer.WriteLine($"Prix total de la facture : {facture.PrixTotal:C}");
                    }
                    float prixTotal = commandes.Sum(f => f.PrixTotal);
                    writer.WriteLine($"Prix total de la commande : {prixTotal:C}");
                }
                Console.WriteLine($"Fichier de commande enregistré avec succès : {fileName}");
            }
            else
            {
                Console.WriteLine("Aucun article n'a été commandé.");
            }
        }
        private List<Acheteur> LoadAcheteursFromJsonFile()
        {
            string jsonFilePath = "C:\\ProjetC#\\GestionCommandeChocolat\\Projet.Data\\data\\acheteurs.json"; // Mettez le chemin d'accès correct à votre fichier JSON
            List<Acheteur> acheteurs = new List<Acheteur>();

            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                acheteurs = JsonConvert.DeserializeObject<List<Acheteur>>(jsonData);
            }

            return acheteurs;
        }

        private void SaveAcheteursToJsonFile(List<Acheteur> acheteurs)
        {
            string jsonFilePath = "C:\\ProjetC#\\GestionCommandeChocolat\\Projet.Data\\data\\acheteurs.json"; // Mettez le chemin d'accès correct à votre fichier JSON
            string jsonData = JsonConvert.SerializeObject(acheteurs, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
    }

