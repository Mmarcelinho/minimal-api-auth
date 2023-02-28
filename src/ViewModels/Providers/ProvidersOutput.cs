namespace MinimalApi.ViewModels.Providers;

public class ProvidersOutput
{
    public ProvidersOutput(int id, string name, bool active)
    {
        Id = id;
        Name = name;
        Active = active;
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public bool Active { get; set; }
}
