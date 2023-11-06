using Projet.Models;
using System.Collections.Generic;
using System;

public class Facture
{
    public Guid Id { get; set; }
    public DateTime DateFacturation { get; set; }
    public string TypeFacture { get; set; }
    public List<Article> Articles { get; set; }
    public float PrixTotal { get; set; }
    public string NomAcheteur { get; set; } 
    public string PrenomAcheteur { get; set; } 

    public Facture(Guid id, DateTime dateFacturation, string typeFacture, string nomAcheteur, string prenomAcheteur)
    {
        Id = id;
        DateFacturation = dateFacturation;
        TypeFacture = typeFacture;
        Articles = new List<Article>();
        NomAcheteur = nomAcheteur; 
        PrenomAcheteur = prenomAcheteur; 
    }
}





