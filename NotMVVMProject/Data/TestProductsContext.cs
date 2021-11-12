using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace NotMVVMProject.Data
{
    public partial class TestProductsContext : DbContext
    {
        public TestProductsContext()
            : base("name=TestProducts")
        {
        }

        public virtual DbSet<Materials> Materials { get; set; }
        public virtual DbSet<MaterialToProduct> MaterialToProduct { get; set; }
        public virtual DbSet<Products> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Materials>()
                .Property(e => e.MaterialName)
                .IsUnicode(false);

            modelBuilder.Entity<Materials>()
                .Property(e => e.Price)
                .IsUnicode(false);

            modelBuilder.Entity<Materials>()
                .HasMany(e => e.MaterialToProduct)
                .WithRequired(e => e.Materials)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MaterialToProduct>()
                .Property(e => e.ProductName)
                .IsUnicode(false);

            modelBuilder.Entity<MaterialToProduct>()
                .Property(e => e.MaterialName)
                .IsUnicode(false);

            modelBuilder.Entity<Products>()
                .Property(e => e.ProductName)
                .IsUnicode(false);

            modelBuilder.Entity<Products>()
                .Property(e => e.Supplier)
                .IsUnicode(false);

            modelBuilder.Entity<Products>()
                .Property(e => e.Type)
                .IsUnicode(false);
        }
    }
}
