using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Product
    {
        [Key]
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int product_price { get; set; }
        public string product_description { get; set; }
        public string product_image { get; set; }
        public int cart_id { get; set; }

        public Category Category { get; set; } //reference of product model to category model 1 category has many products
    }
}
