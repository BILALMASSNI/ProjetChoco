using Projet.FileServices;
using Projet.ListServices;
using Projet.LogServices;
using Projet.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Projet.Core
{
    public class ApplicationManager
    {
        private readonly IAdminService adminService;
        private readonly IAcheteurService acheteurService;
        private readonly IArticleService articleService;
        private readonly ICommandeService commandeService;
        private readonly ILogService logService;
        private List<Commande> commandes;
        private List<Article> articles;
        private List<Acheteur> acheteurs;

        public ApplicationManager(
            IAdminService adminService,
            IAcheteurService acheteurService,
            IArticleService articleService,
            ICommandeService commandeService,
            ILogService logService)
        {
            this.adminService = adminService;
            this.acheteurService = acheteurService;
            this.articleService = articleService;
            this.commandeService = commandeService;
            this.logService = logService;
            commandes = new List<Commande>();
            articles = new List<Article>();
            acheteurs = new List<Acheteur>();
        }

        // ... Autres méthodes d'authentification et de gestion des articles ...

        // Méthode pour créer des fichiers de factures pour un acheteur
        public async Task CreerFactureAcheteurAsync(Acheteur acheteur, List<Article> articles)
        {
            // Demandez à l'acheteur de sélectionner des articles et de saisir les quantités.
            // Enregistrez les sélections dans une liste de commandes.

            List<Commande> commandes = new List<Commande>();

            Console.WriteLine("Sélectionnez des articles (1: Article1, 2: Article2, 3: Article3, ...) ou tapez 'F' pour finir.");

            while (true)
            {
                string choix = Console.ReadLine();

                if (choix == "F")
                {
                    break; // L'acheteur a terminé la sélection.
                }

                if (int.TryParse(choix, out int articleId))
                {
                    Console.Write("Saisissez la quantité : ");
                    if (int.TryParse(Console.ReadLine(), out int quantite))
                    {
                        Guid articleUniqueId = Guid.NewGuid();

                        // Assurez-vous que acheteur.Id est correctement initialisé en tant que Guid
                        if (Guid.TryParse(acheteur.Id.ToString(), out Guid acheteurGuid))
                        {
                            Commande commande = new Commande(acheteurGuid, articleUniqueId, quantite, DateTime.Now);
                            commandes.Add(commande);
                        }
                        else
                        {
                            Console.WriteLine("Identifiant de l'acheteur non valide. Veuillez saisir un identifiant de type Guid.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Quantité non valide. Veuillez saisir un nombre entier.");
                    }
                }
                else
                {
                    Console.WriteLine("Sélection non valide. Veuillez sélectionner un article valide ou taper 'F' pour finir.");
                }
            }

            // Calculez le prix total de la commande.
            float prixTotal = await commandeService.CalculerPrixTotalAsync(commandes, articles);

            // Appelez la méthode de votre service de commande pour générer la facture.
            await commandeService.GenererFactureAsync(acheteur.Nom, acheteur.Prenom, commandes, articles);

            Console.WriteLine($"Facture générée pour {acheteur.Nom} {acheteur.Prenom}. Prix total : {prixTotal:C}");
        }

        // ... Autres méthodes de gestion d'articles et d'authentification admin ...

        // Méthode pour démarrer l'application
        public async Task StartApplicationAsync()
        {
            Console.WriteLine("Veuillez choisir le mode d'utilisation (1: Administrateur, 2: Utilisateur)");
            string mode = Console.ReadLine();

            IFactureService factureService = null;

            if (mode == "1")
            {
                // Injectez les dépendances appropriées ici
                ILogService logService = new LogService(); // Exemple de création d'une instance de logService
                IFileReader<Commande> fileReader = new FileReader<Commande>();
                IFileWriter<Commande> fileWriter = new FileWriter<Commande>();

                var commandeService = new CommandeService(logService, fileReader, fileWriter);

                factureService = new FactureService(logService, commandes, articles, acheteurs);
                var adminMode = new AdminMode(adminService, factureService, commandeService);
                await adminMode.RunAsync();
            }
            else if (mode == "2")
            {
                // Créez une instance de FactureService en passant les bonnes dépendances
                factureService = new FactureService(logService, commandes, articles, acheteurs);

                // instance de ArticleService en passant les dépendances requises
                IFileReader<Article> fileReader = new FileReader<Article>();
                IFileWriter<Article> fileWriter = new FileWriter<Article>();
                var articleService = new ArticleService(logService, fileReader, fileWriter);

                var userMode = new UserMode(factureService, articleService);
                await userMode.RunAsync();
            }
            else
            {
                Console.WriteLine("Mode non valide. L'application se termine.");
            }
        }


    }
}
