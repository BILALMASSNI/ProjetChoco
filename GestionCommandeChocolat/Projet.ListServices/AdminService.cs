using Projet.FileServices;
using Projet.LogServices;
using Projet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projet.ListServices
{
    public class AdminService : IAdminService
    {
        private readonly IFileReader<Administrateur> FileReader;
        private readonly IFileWriter<Administrateur> FileWriter;
        private readonly ILogService logService;
        private List<Administrateur> administrateurs;
        private const string adminDataFilePath = "C:\\ProjetC#\\GestionCommandeChocolat\\Projet.Data\\data\\administrateurs.json"; // Spécifiez le chemin du fichier JSON ici.

        public AdminService(ILogService logService, IFileReader<Administrateur> fileReader, IFileWriter<Administrateur> fileWriter)
        {
            this.logService = logService;
            FileReader = fileReader;
            FileWriter = fileWriter;
            administrateurs = new List<Administrateur>();
            LoadAdministrateursAsync().Wait();
        }
        private async Task LoadAdministrateursAsync()
        {
            var loadedAdmins = await FileReader.ReadDataFromFileAsync(adminDataFilePath);
            if (loadedAdmins != null)
            {
                administrateurs.AddRange(loadedAdmins);
            }
        }
        public async Task<bool> AjouterAdministrateurAsync(Administrateur admin)
        {
            if (administrateurs.Exists(a => a.Login == admin.Login))
            {
                return false;
            }
            administrateurs.Add(admin);
            await SauvegarderAdministrateursAsync();
            await logService.EnregistrerLogAsync($"Ajout d'un administrateur : {admin.Login}");
            return true;
        }
        public Task<bool> VerifierAuthentificationAsync(string login, string password)
        {
            if (administrateurs != null)
            {
                var admin = administrateurs.Find(a => a.Login == login);

                if (admin != null && admin.Password != null && string.Equals(admin.Password, password, StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }



        private async Task SauvegarderAdministrateursAsync()
        {
            await FileWriter.WriteDataToFileAsync(adminDataFilePath, administrateurs);
        }
    }
}
