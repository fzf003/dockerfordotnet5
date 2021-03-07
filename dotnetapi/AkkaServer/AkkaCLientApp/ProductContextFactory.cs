using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaCLientApp
{
    public class ProductContextFactory : IDesignTimeDbContextFactory<ProductTinyContext>
    {
        public ProductTinyContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductTinyContext>();
            optionsBuilder.UseSqlServer("server=.,14330;Initial Catalog=fzf0031;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true");
            //.UseSqlite("Data Source=blog.db");
            return new ProductTinyContext(optionsBuilder.Options);
        }
    }
}
