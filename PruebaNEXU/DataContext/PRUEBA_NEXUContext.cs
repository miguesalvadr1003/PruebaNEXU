using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PruebaNEXU.DataContext
{
    public partial class PRUEBA_NEXUContext : DbContext
    {
        public PRUEBA_NEXUContext()
        {
        }

        public PRUEBA_NEXUContext(DbContextOptions<PRUEBA_NEXUContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<BrandsWithAveragePrice> BrandsWithAveragePrices { get; set; }
        public virtual DbSet<Model> Models { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Database=PRUEBA_NEXU;Username=postgres;Password=1234");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Spanish_Mexico.1252");

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => e.Nombre, "nombre_unico")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<BrandsWithAveragePrice>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("brands_with_average_price");

                entity.Property(e => e.AveragePrice).HasColumnName("average_price");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.ToTable("models");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.AveragePrice).HasColumnName("average_price");

                entity.Property(e => e.BrandName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("brand_name");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.HasOne(d => d.BrandNameNavigation)
                    .WithMany(p => p.Models)
                    .HasPrincipalKey(p => p.Nombre)
                    .HasForeignKey(d => d.BrandName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
