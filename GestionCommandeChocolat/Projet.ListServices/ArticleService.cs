using Projet.FileServices;
using Projet.LogServices;
using Projet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projet.ListServices
{
    public class ArticleService : IArticleService
    {
        private readonly IFileReader<Article> FileReader;
        private readonly IFileWriter<Article> FileWriter;
        private List<Article> articles;
        private const string articleDataFilePath = "C:\\ProjetC#\\GestionCommandeChocolat\\Projet.Data\\data\\articles.json";
        private readonly ILogService logService;

        public ArticleService(ILogService logService, IFileReader<Article> fileReader, IFileWriter<Article> fileWriter)
        {
            this.logService = logService;
            this.FileReader = fileReader;
            this.FileWriter = fileWriter;
            articles = new List<Article>();
            InitializeAsync().Wait();
        }
        public async Task InitializeAsync()
        {
            await LoadArticlesAsync();
            // Autres initialisations si nécessaire
        }

        public async Task<List<Article>> GetArticlesAsync()
        {
            var loadedArticles = await LoadArticlesAsync(); // Chargement des articles à partir du fichier JSON
            return loadedArticles;
        }

        private async Task<List<Article>> LoadArticlesAsync()
        {
            if (FileReader != null)
            {
                var loadedArticles = await FileReader.ReadDataFromFileAsync(articleDataFilePath);
                if (loadedArticles != null)
                {
                    articles.AddRange(loadedArticles);
                }
                return loadedArticles;
            }
            else
            {
                return new List<Article>();
            }
        }


        public async Task<bool> AjouterArticleAsync(Article article)
        {
            articles.Add(article);
            await SauvegarderArticlesAsync();
            await logService.EnregistrerLogAsync($"Ajout d'un article : {article.Reference}");
            return true;
        }

        public async Task<bool> MettreAJourArticleAsync(Guid articleId, Article article)
        {
            var existingArticle = articles.Find(a => a.Id == articleId);
            if (existingArticle != null)
            {
                existingArticle.Reference = article.Reference;
                existingArticle.Prix = article.Prix;
                await SauvegarderArticlesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> SupprimerArticleAsync(Guid articleId)
        {
            var existingArticle = articles.Find(a => a.Id == articleId);
            if (existingArticle != null)
            {
                articles.Remove(existingArticle);
                await SauvegarderArticlesAsync();
                return true;
            }
            return false;
        }

        private async Task SauvegarderArticlesAsync()
        {
            await FileWriter.WriteDataToFileAsync(articleDataFilePath, articles);
        }
    }
}
