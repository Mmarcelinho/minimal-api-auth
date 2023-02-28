namespace MinimalApi.ViewModels.Providers;

public class ProvidersInput
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(5, ErrorMessage = "No mínimo 5 caracteres")]
    public string Name { get; set; }

    public bool Active { get; set; }

}
