namespace MinimalApi.Data;

    public class Context : DbContext
    {

        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

        public DbSet<Provider> Provider { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Product> Product { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        modelBuilder.ApplyConfiguration(new ProviderMap());
        modelBuilder.ApplyConfiguration(new CategoryMap());
        modelBuilder.ApplyConfiguration(new ProductMap());

        base.OnModelCreating(modelBuilder);

        }
    }
