using Projet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ListServices
{
    public interface ICommandeService
    {

        Task<float> CalculerPrixTotalAsync(List<Commande> commandes, List<Article> articles);
        Task<bool> GenererFactureAsync(string acheteurNom, string acheteurPrenom, List<Commande> commandes, List<Article> articles);
    }
}
