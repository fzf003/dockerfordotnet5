using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace AkkaCLientApp
{
    public interface IDbContextProvider<out TDbContext> where TDbContext : IDbConnection
    {
        TDbContext CreateContext();
    }

    public class DbContextProvider : IDbContextProvider<DbConnection>
    {
        const string productconnection = "server=.,14330;Initial Catalog=fzf003;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true";

        public DbConnection CreateContext()
        {
            var conn= new SqlConnection(productconnection);
            conn.StateChange += (s,e) => {
                Console.WriteLine(e.OriginalState+"---"+e.CurrentState);
            };

            conn.InfoMessage += (s, e) => {
                Console.WriteLine(e.Message);
            };

           
           // conn.Open(SqlConnectionOverrides.OpenWithoutRetry);
            return conn;
        }
    }


    public class ProductTinyContext : DbContext
    {
       
        public ProductTinyContext(DbContextOptions<ProductTinyContext> options)
          : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           /* modelBuilder.Entity<Blog>(e => {

                e.Property(p => p.BlogId);
                e.Property(p => p.Rating);
                e.Property(p => p.Url);
                e.Property(p => p.Path);

                e.ToTable("t1");
                 //.ToView("blog_view");
            });*/
                
             base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           // string productconnection = "server=.,14330;Initial Catalog=fzf0031;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true";

            
          /* optionsBuilder.UseSqlServer(productconnection, options=> {
                options.EnableRetryOnFailure(5);
           });*/

            optionsBuilder.LogTo(Console.WriteLine);



        }

        public DbSet<Product> Products { get;set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }


    }
}
