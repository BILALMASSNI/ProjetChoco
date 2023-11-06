using System;
using Projet.LogServices;
using Projet.ListServices;
using Projet.Core;
using System.Threading.Tasks;
using Projet.FileServices;
using Projet.Models;

namespace Projet
{
    class Program
    {
        static async Task Main(string[] args)
        {

            IFileReader<Administrateur> fileReader = new FileReader<Administrateur>();
            IFileWriter<Administrateur> fileWriter = new FileWriter<Administrateur>();
            IFileReader<Article> articleFileReader = new FileReader<Article>(); ;
            IFileWriter<Article> articleFileWriter = new FileWriter<Article>();
            IFileReader<Acheteur> acheteurFileReader = new FileReader<Acheteur>();
            IFileWriter<Acheteur> acheteurFileWriter = new FileWriter<Acheteur>();
            IFileReader<Commande> commandeFileReader = new FileReader<Commande>();
            IFileWriter<Commande> commandeFileWriter = new FileWriter<Commande>();
            ILogService logService = new LogService();
            ICommandeService commandeService = new CommandeService(logService, commandeFileReader, commandeFileWriter);
            IArticleService articleService = new ArticleService(logService, articleFileReader, articleFileWriter);
            IAdminService adminService = new AdminService(logService, fileReader, fileWriter);
            IAcheteurService acheteurService = new AcheteurService(logService, acheteurFileReader, acheteurFileWriter);

            ApplicationManager appManager = new ApplicationManager(
                adminService,
                acheteurService,
                articleService,
                commandeService,
                logService
            );

            try
            {
                await appManager.StartApplicationAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
            }
        }
    }
}
