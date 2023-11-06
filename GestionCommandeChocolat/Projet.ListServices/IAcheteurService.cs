using Projet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ListServices
{

    public interface IAcheteurService
    {
        Task<bool> AjouterAcheteurAsync(Acheteur acheteur);
        Task<bool> VerifierAuthentificationAsync(string nom, string prenom, int telephone);
    }

}
