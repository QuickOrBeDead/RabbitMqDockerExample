namespace TransferWorker.Infrastructure
{
    using Microsoft.EntityFrameworkCore;


    public sealed class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }
    }

    public sealed class TransferDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public TransferDbContext(DbContextOptions<TransferDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Customer>().HasKey(m => m.Id);

            builder.Entity<Customer>().HasData(new Customer { Id = 1, Name = "customer1", Balance = 1000 });
            builder.Entity<Customer>().HasData(new Customer { Id = 2, Name = "customer2", Balance = 1000 });

            base.OnModelCreating(builder);
        }
    }
}
