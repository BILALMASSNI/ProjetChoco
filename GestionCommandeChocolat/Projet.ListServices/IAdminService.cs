using Projet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ListServices
{

    public interface IAdminService
    {
        Task<bool> AjouterAdministrateurAsync(Administrateur admin);
        Task<bool> VerifierAuthentificationAsync(string login, string password);
    }
}
