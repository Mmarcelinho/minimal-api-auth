namespace MinimalApi.ViewModels.Products;

public class ProductsOutput
{
    public ProductsOutput(int id, string title, decimal price, DateTime  lastUpdateDate , int providerId, int categoryId)
    {
        Id = id;
        Title = title;
        Price = price;
        LastUpdateDate =  lastUpdateDate ;
        ProviderId = providerId;
        CategoryId = categoryId;
    }

    public int Id { get; set; }

    public string Title { get; set; }

    public decimal Price { get; set; }

      public DateTime LastUpdateDate { get; set; }

    public int CategoryId { get; set; }

    public int ProviderId { get; set; }

}
