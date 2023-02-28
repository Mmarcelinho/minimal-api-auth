namespace MinimalApi.Models;

    public class Product
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public decimal Price { get; set; }

        public DateTime InputDate { get; set; }

        public int CategoryId { get; set; } 

        public int ProviderId { get; set; }

        public Category Category { get; set; } = null!;

        public Provider Provider { get; set; } = null!;
        
    }
