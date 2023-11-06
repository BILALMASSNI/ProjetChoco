using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Projet.Models
{
    public class Article
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public float Prix { get; set; }

        public Article(Guid id, string reference, float prix)
        {
            Id = id;
            Reference = reference;
            Prix = prix;
        }
    }
}