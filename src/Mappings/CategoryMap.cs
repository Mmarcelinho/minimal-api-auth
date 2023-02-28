namespace MinimalApi.Mappings;

public class CategoryMap : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
        .IsRequired()
        .HasColumnType("varchar(200)");

        builder.ToTable("Categories");
    }
}
