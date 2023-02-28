namespace MinimalApi.Mappings;
public class ProviderMap : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
        .IsRequired()
        .HasColumnType("varchar(200)");

        builder.ToTable("Providers");
    }
}
