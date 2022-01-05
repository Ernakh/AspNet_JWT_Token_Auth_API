using AulaRestAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AulaRestAPI.Data
{
    public class Contexto : DbContext
    {
        public DbSet<Pessoa> Pessoas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            opt.UseSqlServer(@"Data Source=localhost;initial Catalog=API_Aula;User ID=usuario;password=senha;language=Portuguese;Trusted_Connection=True;");
        }
    }
}
