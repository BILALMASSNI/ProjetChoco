using Projet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ListServices
{
    public interface IFactureService
    {
        Task CreerFacturePourAcheteurAsync(Acheteur acheteur, List<Article> articles);
        Task<bool> EnregistrerFactureAsync(Facture facture);
        Task<Facture> GenererFactureParAcheteurAsync(Guid acheteurId );
        Task<object> GenererFactureParDateAchat(DateTime dateAchat);
    }
}
