using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Category
    {
        [Key]
        public int category_id { get; set; }
        public string category_name { get; set; }
         
        public List<Product> Product { get; set; } //reference of category model to product model, 1 category has many products thats why in list

    }
}
