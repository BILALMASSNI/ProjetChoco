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
    public class AdminMode
    {
        private readonly IAdminService adminService;
        private readonly IFactureService factureService;
        private readonly CommandeService commandeService;

        public AdminMode(IAdminService adminService, IFactureService factureService, CommandeService commandeService)
        {
            this.adminService = adminService;
            this.factureService = factureService;
            this.commandeService = commandeService;
        }

        // Méthode pour exécuter le mode administrateur
        public async Task RunAsync()
        {
            bool isAuthenticationSuccess = false;

            while (!isAuthenticationSuccess)
            {
                Console.Write("Nom d'administrateur : ");
                string username = Console.ReadLine();
                Console.Write("Mot de passe : ");
                string password = Console.ReadLine();

                isAuthenticationSuccess = await adminService.VerifierAuthentificationAsync(username, password);

                if (!isAuthenticationSuccess)
                {
                    Console.WriteLine("Échec de l'authentification. Le nom d'administrateur ou le mot de passe est incorrect.");
                    Console.WriteLine("Voulez-vous ajouter un nouvel administrateur ? (Oui/Non)");
                    string reponse = Console.ReadLine();
                    if (reponse.Equals("Oui", StringComparison.OrdinalIgnoreCase))
                    {
                        AjouterNouvelAdministrateur();
                    }
                }
                else
                {
                    Console.WriteLine("Authentification réussie. Bienvenue, administrateur!");
                }
            }


            while (isAuthenticationSuccess)
            {
                Console.WriteLine("1. Ajouter un nouvel article");
                Console.WriteLine("2. Générer une facture par acheteur");
                Console.WriteLine("3. Générer une facture par date d'achat");
                Console.WriteLine("4. Générer une facture de somme des articles vendus");
                Console.WriteLine("5. Quitter le mode administrateur");

                string adminOption = Console.ReadLine();

                if (adminOption == "1")
                {
                    AjouterNouvelArticle();
                }
                else if (adminOption == "2")
                {
                    try
                    {
                        await GenererFactureParAcheteur();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur lors de la génération de la facture : " + ex.Message);
                    }
                }
                else if (adminOption == "3")
                {
                    await GenererFactureParDateAchat();
                }
                else if (adminOption == "4")
                {
                    await GenererFactureSommeArticlesVendus();
                }
                else if (adminOption == "5")
                {
                    Console.WriteLine("Sortie du mode administrateur.");
                    isAuthenticationSuccess = false;
                }
                else
                {
                    Console.WriteLine("Option non valide. Veuillez sélectionner une option valide.");
                }
            }
        }
        private void AjouterNouvelAdministrateur()
        {
            Console.Write("Nom  du nouvel administrateur : ");
            string username = Console.ReadLine();
            Console.Write("Mot de passe du nouvel administrateur : ");
            string password = Console.ReadLine();

            Guid nouvelId = Guid.NewGuid();
            Administrateur nouvelAdmin = new Administrateur(nouvelId, username, password);

            string jsonFilePath = "C:\\ProjetC#\\GestionCommandeChocolat\\Projet.Data\\data\\administrateurs.json";

            List<Administrateur> administrateurs = new List<Administrateur>();

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                administrateurs = JsonConvert.DeserializeObject<List<Administrateur>>(jsonContent);
            }

            administrateurs.Add(nouvelAdmin);

            string jsonContentUpdated = JsonConvert.SerializeObject(administrateurs, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonContentUpdated);

            Console.WriteLine("Nouvel administrateur ajouté au fichier JSON.");
        }

        // Méthode pour ajouter un nouvel article
        private void AjouterNouvelArticle()
        {
            Console.Write("Nom de l'article : ");
            string nomArticle = Console.ReadLine();

            Console.Write("Prix de l'article : ");
            if (float.TryParse(Console.ReadLine(), out float prixArticle))
            {
                Guid nouvelId = Guid.NewGuid();

                Article nouvelArticle = new Article(nouvelId, nomArticle, prixArticle);

                string jsonFilePath = @"C:\ProjetC#\GestionCommandeChocolat\Projet.Data\data\articles.json";
                List<Article> articles = new List<Article>();

                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    articles = JsonConvert.DeserializeObject<List<Article>>(jsonContent);
                }

                articles.Add(nouvelArticle);

                string jsonContentUpdated = JsonConvert.SerializeObject(articles, Formatting.Indented);
                File.WriteAllText(jsonFilePath, jsonContentUpdated);

                Console.WriteLine("Nouvel article ajouté au fichier JSON.");
            }
            else
            {
                Console.WriteLine("Prix non valide. Veuillez saisir un nombre à virgule flottante.");
            }
        }
       
        private async Task GenererFactureParDateAchat()
        {
            Console.Write("Date d'achat (AAAA-MM-JJ) : ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dateAchat))
            {
                object factureObj = await factureService.GenererFactureParDateAchat(dateAchat);

                if (factureObj != null && factureObj is Facture facture)
                {
                    Console.WriteLine("Facture générée avec succès !");
                    Console.WriteLine("Détails de la facture :");
                    Console.WriteLine("ID de la facture : " + facture.Id);
                    Console.WriteLine("Date de la facture : " + facture.DateFacturation);
                    Console.WriteLine("Type de facture : " + facture.TypeFacture);

                    Console.WriteLine("Articles de la facture :");
                    foreach (var article in facture.Articles)
                    {
                        Console.WriteLine($" - {article.Reference} : {article.Prix:C}");
                    }

                    Console.WriteLine($"Prix total de la facture : {facture.PrixTotal:C}");
                }
                else
                {
                    Console.WriteLine("Erreur lors de la génération de la facture.");
                }
            }
            else
            {
                Console.WriteLine("Date d'achat non valide. Veuillez saisir une date au format AAAA-MM-JJ.");
            }
        }

        private async Task GenererFactureParAcheteur()
        {
            Console.Write("ID de l'acheteur : ");
            if (Guid.TryParse(Console.ReadLine(), out Guid acheteurId))
            {
                Facture facture = await factureService.GenererFactureParAcheteurAsync(acheteurId);

                if (facture != null)
                {
                    Console.WriteLine("Facture générée avec succès !");
                    Console.WriteLine("Détails de la facture :");
                    Console.WriteLine("ID de la facture : " + facture.Id);
                    Console.WriteLine("Date de la facture : " + facture.DateFacturation);
                    Console.WriteLine("Type de facture : " + facture.TypeFacture);

                    Console.WriteLine("Articles de la facture :");
                    foreach (var article in facture.Articles)
                    {
                        Console.WriteLine($" - {article.Reference} : {article.Prix:C}");
                    }

                    Console.WriteLine($"Prix total de la facture : {facture.PrixTotal:C}");
                }
                else
                {
                    Console.WriteLine("Aucune facture n'a été générée pour cet acheteur.");
                }
            }
            else
            {
                Console.WriteLine("ID de l'acheteur non valide. Veuillez saisir un identifiant de type Guid.");
            }
        }

       


        private async Task GenererFactureSommeArticlesVendus()
        {
            var articlesVendus = await commandeService.ObtenirArticlesVendusAsync();

            if (articlesVendus != null && articlesVendus.Any())
            {
                var facture = new Facture(Guid.NewGuid(), DateTime.Now, "Récapitulative", "Nom de l'acheteur", "Prénom de l'acheteur");
                facture.Articles.AddRange(articlesVendus);

                float prixTotal = facture.Articles.Sum(article => article.Prix);
                facture.PrixTotal = prixTotal;

                bool factureEnregistree = await factureService.EnregistrerFactureAsync(facture);

                if (factureEnregistree)
                {
                    Console.WriteLine("Facture récapitulative générée avec succès.");
                    Console.WriteLine("Détails de la facture :");
                    Console.WriteLine("ID de la facture : " + facture.Id);
                    Console.WriteLine("Date de la facture : " + facture.DateFacturation);
                    Console.WriteLine("Type de facture : " + facture.TypeFacture);
                    Console.WriteLine($"Prix total de la facture : {prixTotal:C}");
                }
                else
                {
                    Console.WriteLine("Erreur lors de l'enregistrement de la facture récapitulative.");
                }
            }
            else
            {
                Console.WriteLine("Aucun article vendu trouvé pour générer la facture récapitulative.");
            }
        }
    }
}
