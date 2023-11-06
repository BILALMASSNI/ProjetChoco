using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.Models
{
    public class Administrateur
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public Administrateur(Guid id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
        }
    }
}