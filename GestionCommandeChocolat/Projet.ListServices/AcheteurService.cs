using Projet.FileServices;
using Projet.LogServices;
using Projet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ListServices
{
    public class AcheteurService : IAcheteurService
    {
        private readonly IFileReader<Acheteur> FileReader;
        private readonly IFileWriter<Acheteur> FileWriter;
        private List<Acheteur> acheteurs;
        private const string acheteurDataFilePath = "C:\\ProjetC#\\GestionCommandeChocolat\\Projet.Data\\data\\acheteurs.json"; // Spécifiez le chemin du fichier JSON ici.
        private readonly ILogService logService;

        public AcheteurService(ILogService logService)
        {
            this.logService = logService;
        }
        private readonly IFileReader<Acheteur> acheteurFileReader;
        private readonly IFileWriter<Acheteur> acheteurFileWriter;
        public AcheteurService()
        {
            acheteurs = new List<Acheteur>();
        }
        public AcheteurService(ILogService logService, IFileReader<Acheteur> acheteurFileReader, IFileWriter<Acheteur> acheteurFileWriter)
        {
            // Initialisation les propriétés ou dépendances avec les paramètres
            this.logService = logService;
            this.acheteurFileReader = acheteurFileReader;
            this.acheteurFileWriter = acheteurFileWriter;
        }
        public AcheteurService(IFileReader<Acheteur> fileReader, IFileWriter<Acheteur> fileWriter)
        {
            FileReader = fileReader;
            FileWriter = fileWriter;
            acheteurs = new List<Acheteur>();
            LoadAcheteursAsync().Wait();
        }
        public async Task<bool> AjouterAcheteurAsync(Acheteur acheteur)
        {
            if (acheteurs.Exists(a => a.Nom == acheteur.Nom && a.Prenom == acheteur.Prenom && a.Telephone == acheteur.Telephone))
            {
                return false;
            }
            acheteurs.Add(acheteur);
            await SauvegarderAcheteursAsync();
            await logService.EnregistrerLogAsync($"Ajout d'un acheteur : {acheteur.Nom} {acheteur.Prenom}");
            return true;
        }
        public Task<bool> VerifierAuthentificationAsync(string nom, string prenom, int telephone)
        {
            // Vérification si l'acheteur existe en utilisant la méthode Exists
            if (acheteurs.Exists(a => a.Nom == nom && a.Prenom == prenom && a.Telephone == telephone))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        private async Task LoadAcheteursAsync()
        {
            var loadedAcheteurs = await FileReader.ReadDataFromFileAsync(acheteurDataFilePath);
            if (loadedAcheteurs != null)
            {
                acheteurs.AddRange(loadedAcheteurs);
            }
        }
        private async Task SauvegarderAcheteursAsync()
        {
            await FileWriter.WriteDataToFileAsync(acheteurDataFilePath, acheteurs);
        }
    }

}
