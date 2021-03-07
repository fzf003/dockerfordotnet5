 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AkkaCLientApp
{
    [Table("t1")]
    public class Product
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string ImageFile { get; set; }

        public decimal Price { get; set; }

        public int Status { get; set; }
        // [Dapper.Contrib.Extensions.Write(false)]
        public DateTime CreateTime { get; set; }

       // public List<Blog> Blogs { get; set; } 

        public Product()
        {
           // this.Blogs = new List<Blog>();
        }
    }

    [Table("Blogs")]
    public class Blog
    {
       
        public int BlogId { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }

        public string Path { get; set; }
    }

    [Table("Users")]
    public class User
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public int Age { get; set; }

        public User()
        {
            this.Roles = new List<Role>();
        }
        //public long RoleId { get; set; }
       // [NotMapped]
        public   List<Role> Roles { get; set; }
    }

    [Table("Roles")]
    public class Role
    {
        public Role(string name)
        {
             
            Name = name;
        }

        public Role ChangeName(string name)
        {
            this.Name = name;
            return this;
        }

        [Key]
        public long Id { get; private set; }

        public string Name { get; private set; }
    }
     
    public class UserRole
    {
        public User User { get; set; }

        public Role Role { get;
        set;}
    }

}
