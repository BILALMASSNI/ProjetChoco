using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Projet.Models;

namespace Projet.ListServices
{
    public interface IArticleService
    {
        Task<List<Article>> GetArticlesAsync(); // Ajoutez cette méthode pour récupérer la liste d'articles
        Task<bool> AjouterArticleAsync(Article article);
        Task<bool> MettreAJourArticleAsync(Guid articleId, Article article);
       
        Task<bool> SupprimerArticleAsync(Guid articleId);
    }
}