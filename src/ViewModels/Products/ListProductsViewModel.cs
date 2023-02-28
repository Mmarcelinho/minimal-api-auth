namespace MinimalApi.ViewModels.Products;

public class ListProductsViewModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public decimal Price { get; set; }

    public DateTime LastUpdateDate { get; set; }

    public string Category { get; set; }

    public string Provider { get; set; }

}
