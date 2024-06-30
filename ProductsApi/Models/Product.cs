using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Condtion { get; set; }

        public string? Image { get; set; }
        public int Price { get; set; }
        public int? Discount { get; set; }
        public string? Category { get; set; }

    }
}
