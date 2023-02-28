namespace MinimalApi.ViewModels.Products;

public class ProductsInput
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(5, ErrorMessage = "No mínimo 5 caracteres")]
    public string Title { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Campo de valor obrigátorio")]
    public decimal Price { get; set; }

     public DateTime LastUpdateDate { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int ProviderId { get; set; }
}
