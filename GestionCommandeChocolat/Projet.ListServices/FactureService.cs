using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Projet.ListServices;
using Projet.LogServices;
using Projet.Models;



public class FactureService : IFactureService
{
    private readonly ILogService logService;
    private readonly List<Commande> commandes;
    private readonly List<Article> articles;
    private readonly List<Acheteur> acheteurs;

    public FactureService(ILogService logService, List<Commande> commandes, List<Article> articles, List<Acheteur> acheteurs)
    {
        this.logService = logService;
        this.commandes = commandes;
        this.articles = articles;
        this.acheteurs = acheteurs;
    }   
    public async Task<bool> EnregistrerFactureAsync(Facture facture)
    {
        try
        {
            string fileName = $"C:\\Users\\hassania\\OneDrive\\Bureau\\Nouveau dossier (5)\\Facture-{facture.Id}-{DateTime.Now:dd-MM-yyyy-HH-mm}.txt";

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"Facture pour {facture.NomAcheteur} {facture.PrenomAcheteur}");
                writer.WriteLine($"Date de la facture : {facture.DateFacturation}");
                writer.WriteLine("Articles achetés :");

                // l'Ajoute de la liste des articles achetés
                foreach (var article in facture.Articles)
                {
                    writer.WriteLine($" - {article.Reference} : {article.Prix:C}");
                }

                writer.WriteLine($"Montant total : {facture.PrixTotal:C}");
                Console.WriteLine($"Facture enregistrée avec succès : {fileName}");
            }

            await logService.EnregistrerLogAsync($"Facture enregistrée pour {facture.NomAcheteur} {facture.PrenomAcheteur}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de l'enregistrement de la facture : " + ex.Message);
            return false;
        }
    }

    // Méthode pour calculer la somme des articles vendus
    public float CalculerSommeArticlesVendus()
    {
        float somme = 0.0f;

        foreach (var commande in commandes)
        {
            var article = articles.Find(a => a.Id == commande.IdChocolat);
            if (article != null)
            {
                somme += article.Prix * commande.Quantite;
            }
        }

        return somme;
    }
    // Méthode pour calculer la somme des articles vendus par acheteurs
    public async Task<float> CalculerSommeArticlesVendusParAcheteursAsync()
    {
        float somme = 0.0f;
        var commandesParAcheteur = commandes.GroupBy(c => c.IdAcheteur);
        //  liste de tâches pour calculer la somme de chaque acheteur en parallèle
        var calculTasks = new List<Task<float>>();
        foreach (var groupe in commandesParAcheteur)
        {
            var acheteur = acheteurs.FirstOrDefault(a => a.Id == groupe.Key);
            if (acheteur != null)
            {
                // Task.Run pour paralléliser le calcul de somme d'acheteur
                Task<float> calculTask = Task.Run(() =>
                {
                    float sommeAcheteur = groupe.Sum(c => c.Quantite);
                    return sommeAcheteur;
                });
                calculTasks.Add(calculTask);
            }
        }
        await Task.WhenAll(calculTasks);
        somme = calculTasks.Sum(task => task.Result);
        return somme;
    }
    // Méthode pour calculer la somme des articles vendus par date d'achat
    public async Task<float> CalculerSommeArticlesVendusParDateAchatAsync()
    {
        var commandesParDate = commandes.GroupBy(c => c.DateAchat.Date);

        var sommeTasks = commandesParDate.Select(groupe =>
        {
            return Task.Run(() =>
            {
                float sommeDate = groupe.Sum(c => c.Quantite);
                return sommeDate;
            });
        });
        float[] results = await Task.WhenAll(sommeTasks);
        float somme = results.Sum();
        return somme;
    }
    public async Task CreerFacturePourAcheteurAsync(Acheteur acheteur, List<Article> articles)
    {
        if (articles == null || commandes == null)
        {
            Console.WriteLine("Les données d'articles ou de commandes ne sont pas disponibles.");
            return; // Ou effectuez une autre gestion d'erreur appropriée
        }
        //  une nouvelle instance de Facture
        Facture nouvelleFacture = new Facture(Guid.NewGuid(), DateTime.Now, "Facture d'achat", acheteur.Nom, acheteur.Prenom);
        // Récupération des articles achetés par cet acheteur
        List<Article> articlesAchetes = new List<Article>();
        foreach (var commande in commandes)
        {
            if (commande.IdAcheteur == acheteur.Id)
            {
                Article article = articles.FirstOrDefault(a => a.Id == commande.IdChocolat);

                if (article != null)
                {
                    articlesAchetes.Add(article);
                    Console.WriteLine($"Article ajouté : {article.Reference}");
                }
            }
        }
        if (articlesAchetes.Count == 0)
        {
            Console.WriteLine("Cet acheteur n'a effectué aucun achat.");
            return; 
        }
        // Calcule de  prix total
        float prixTotal = articlesAchetes.Sum(article => article.Prix);
        nouvelleFacture.PrixTotal = prixTotal;
        // Enregistrement de la facture dans un fichier texte
        bool enregistrementReussi = await EnregistrerFactureAsync(nouvelleFacture);
        if (enregistrementReussi)
        {
            Console.WriteLine($"Facture générée avec succès pour {acheteur.Nom} {acheteur.Prenom}");
        }
        else
        {
            Console.WriteLine("Erreur lors de l'enregistrement de la facture.");
        }
    }
    public async Task<Facture> GenererFactureParAcheteurAsync(Guid acheteurId)
    {    
       Acheteur acheteur = acheteurs.FirstOrDefault(a => a.Id == acheteurId);
        if (acheteur == null)
        {
      
            return null;
        }
        // Création d'une nouvelle instance de Facture
        Facture facture = new Facture(Guid.NewGuid(), DateTime.Now, "Facture d'achat", acheteur.Nom, acheteur.Prenom);
        // Récupérez les articles achetés par cet acheteur
        List<Article> articlesAchetes = new List<Article>();
        foreach (var commande in commandes)
        {
            if (commande.IdAcheteur == acheteurId)
            {
                Article article = articles.FirstOrDefault(a => a.Id == commande.IdChocolat);

                if (article != null)
                {
                    articlesAchetes.Add(article);
                }
            }
        }

        // Ajoutez les articles à la facture
        facture.Articles.AddRange(articlesAchetes);

        // Calculez le prix total
        float prixTotal = articlesAchetes.Sum(article => article.Prix);
        facture.PrixTotal = prixTotal;

        // Opération asynchrone factice pour satisfaire le compilateur
        await Task.CompletedTask;

        return facture;
    }



    public async Task<object> GenererFactureParDateAchat(DateTime dateAchat)
    {
        try
        {
            // Filtre des commandes passées à la date spécifiée
            var commandesPourDate = commandes.Where(c => c.DateAchat.Date == dateAchat.Date).ToList();

            if (commandesPourDate.Count == 0)
            {
                Console.WriteLine("Aucune commande trouvée à la date spécifiée.");
                return null; // Ou renvoyez un objet approprié pour indiquer l'absence de commandes
            }

            // Création une nouvelle instance de Facture
            Facture facture = new Facture(Guid.NewGuid(), DateTime.Now, "Facture par date d'achat", "Nom de l'acheteur", "Prénom de l'acheteur");

            // l'Ajoute des articles correspondant aux commandes à la facture
            foreach (var commande in commandesPourDate)
            {
                var article = articles.FirstOrDefault(a => a.Id == commande.IdChocolat);
                if (article != null)
                {
                    facture.Articles.Add(article);
                }
            }

            // Calcule de prix total
            float prixTotal = facture.Articles.Sum(article => article.Prix);
            facture.PrixTotal = prixTotal;

            // Enregistrement de la facture dans un fichier texte
            bool enregistrementReussi = await EnregistrerFactureAsync(facture);

            if (enregistrementReussi)
            {
                Console.WriteLine($"Facture par date d'achat générée avec succès pour le {dateAchat:dd-MM-yyyy}");
                await logService.EnregistrerLogAsync($"Facture par date d'achat générée pour le {dateAchat:dd-MM-yyyy}");
            }
            else
            {
                Console.WriteLine("Erreur lors de l'enregistrement de la facture par date d'achat.");
                return null; 
            }

            // la facture générée ou un autre objet approprié
            return facture;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de la génération de la facture par date d'achat : " + ex.Message);
            return null; 
        }
    }




}
