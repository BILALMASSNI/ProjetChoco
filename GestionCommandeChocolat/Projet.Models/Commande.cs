using System;

namespace Projet.Models
{
    public class Commande
    {
        public Guid IdAcheteur { get; set; }
        public Guid IdChocolat { get; set; }
        public int Quantite { get; set; }
        public DateTime DateAchat { get; set; }

        public Commande(Guid idAcheteur, Guid idChocolat, int quantite, DateTime dateAchat)
        {
            IdAcheteur = idAcheteur;
            IdChocolat = idChocolat;
            Quantite = quantite;
            DateAchat = dateAchat;
        }
    }
}
