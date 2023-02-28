namespace MinimalApi.ViewModels.Categories;

public class CategoriesInput
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(5, ErrorMessage = "No mínimo 5 caracteres")]
    public string Title { get; set; }
}
