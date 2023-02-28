namespace MinimalApi.Models;

public class Provider
{
  
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool Active { get; set; }

    public IList<Product>? Products { get; set; }
}
