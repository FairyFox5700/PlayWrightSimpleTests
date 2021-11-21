namespace PlayWrightNUnit.Models
{
    public class Product
    {
        public Product()
        {

        }
        public Product(string name, decimal price, string description, string category)
        {
            Name = name;
            Price = price;
            Description = description;
            Category = category;
        }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

    }
}