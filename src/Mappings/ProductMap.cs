namespace MinimalApi.Mappings;

public class ProductMap : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
        .IsRequired()
        .HasColumnType("varchar(200)");

        builder.Property(p => p.Price)
        .IsRequired()
        .HasColumnType("numeric(38,2)");

        builder.Property(p => p.InputDate)
        .HasColumnType("datetime");

        builder.HasOne(P => P.Category)
        .WithMany(p => p.Products)
        .HasForeignKey(p => p.CategoryId)
        .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(P => P.Provider)
        .WithMany(p => p.Products)
        .HasForeignKey(p => p.ProviderId)
        .OnDelete(DeleteBehavior.NoAction);

        builder.ToTable("Products");
    }
}
