using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetaPocoClient
{
    [PetaPoco.TableName("ProductNew")]
    [PetaPoco.PrimaryKey("Id")]
    public class Product
    {
     
        public long Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string ImageFile { get; set; }

        public decimal Price { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
